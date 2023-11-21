using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;

namespace OR_tools
{
    public static class PDPFunction
    {
        [FunctionName("SolvePDP")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Solving PDP using OR-Tools.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            long[,] distanceMatrix = JsonConvert.DeserializeObject<long[,]>(data._distanceMatrix.ToString());
            int[][] pickupsDeliveries = JsonConvert.DeserializeObject<int[][]>(data.pickupsDeliveries.ToString());

            var result = SolvePDP(distanceMatrix, pickupsDeliveries);

            return new OkObjectResult(result);
        }

        private static (List<int>, List<(int, int)>) SolvePDP(long[,] distanceMatrix, int[][] pickupsDeliveries)
        {
            RoutingIndexManager manager = new RoutingIndexManager(distanceMatrix.GetLength(0), 1, 0);
            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return distanceMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            foreach (var pair in pickupsDeliveries)
            {
                long pickupNode = manager.NodeToIndex(pair[0]);
                long deliveryNode = manager.NodeToIndex(pair[1]);
                routing.AddPickupAndDelivery(pickupNode, deliveryNode);
            }

            Assignment solution = routing.Solve();

            List<int> route = new List<int>();
            long index = routing.Start(0);
            while (!routing.IsEnd(index))
            {
                route.Add(manager.IndexToNode(index));
                index = solution.Value(routing.NextVar(index));
            }
            route.Add(manager.IndexToNode(index));

            List<(int, int)> pickupDeliveriesResult = new List<(int, int)>();
            foreach (var pair in pickupsDeliveries)
            {
                long pickupNode = manager.NodeToIndex(pair[0]);
                long deliveryNode = manager.NodeToIndex(pair[1]);
                pickupDeliveriesResult.Add((route.IndexOf((int)pickupNode), route.IndexOf((int)deliveryNode)));
            }

            return (route, pickupDeliveriesResult);
        }
    }
}
