using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Don't forget this!

// This controller will live at the route we define.
// [ApiController] is standard for APIs.
// [Route("api/v1/projects/{projectId}")] matches your requirement.
[ApiController]
[Route("api/v1/projects/{projectId}")]
public class SchedulingController : ControllerBase
{
    // This is the endpoint: POST /api/v1/projects/{projectId}/schedule
    [HttpPost("schedule")]
    public IActionResult CreateSchedule([FromRoute] string projectId, [FromBody] ScheduleRequest request)
    {
        // Note: The 'projectId' from the route isn't used, but it's here.
        
        if (request == null || request.Tasks == null || request.Tasks.Count == 0)
        {
            return BadRequest(new { error = "No tasks provided." });
        }

        try
        {
            // Call our core logic function!
            List<string> orderedTasks = GetScheduleOrder(request.Tasks);

            // Format the response to match the output example
            var response = new ScheduleResponse
            {
                RecommendedOrder = orderedTasks
            };
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            // This catches the circular dependency error
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Catch any other unexpected errors
            return StatusCode(500, new { error = "An internal server error occurred.", details = ex.Message });
        }
    }

    //
    // --- THIS IS THE CORE "SMART SCHEDULER" LOGIC ---
    //
    private List<string> GetScheduleOrder(List<TaskItem> tasks)
    {
        // 1. Build the graph (adjacency list) and in-degree counts
        var inDegree = new Dictionary<string, int>();
        var adjList = new Dictionary<string, List<string>>();

        // Initialize dictionaries for all tasks
        foreach (var task in tasks)
        {
            inDegree[task.Title] = 0;
            adjList[task.Title] = new List<string>();
        }

        // Populate the graph and in-degrees based on dependencies
        foreach (var task in tasks)
        {
            foreach (var depTitle in task.Dependencies)
            {
                // Safety check: Does the dependency exist?
                if (!adjList.ContainsKey(depTitle))
                {
                    throw new InvalidOperationException($"Dependency '{depTitle}' for task '{task.Title}' does not exist.");
                }
                
                // If "Implement Backend" depends on "Design API":
                // Add "Implement Backend" to "Design API"'s adjacency list
                adjList[depTitle].Add(task.Title);

                // And increment the in-degree for "Implement Backend"
                inDegree[task.Title]++;
            }
        }

        // 2. Initialize the queue with tasks having 0 dependencies
        var queue = new Queue<string>();
        foreach (var taskTitle in inDegree.Keys)
        {
            if (inDegree[taskTitle] == 0)
            {
                queue.Enqueue(taskTitle);
            }
        }

        // 3. Process the queue
        var recommendedOrder = new List<string>();
        while (queue.Count > 0)
        {
            var currentTaskTitle = queue.Dequeue();
            recommendedOrder.Add(currentTaskTitle);

            // Find neighbors (tasks that depend on the current task)
            foreach (var neighbor in adjList[currentTaskTitle])
            {
                // Decrement their in-degree
                inDegree[neighbor]--;
                
                // If in-degree is 0, add to queue
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        // 4. Finish and check for cycles
        if (recommendedOrder.Count != tasks.Count)
        {
            // We have a cycle! The schedule is impossible.
            throw new InvalidOperationException("A circular dependency was detected. The schedule is impossible.");
        }

        return recommendedOrder;
    }
}