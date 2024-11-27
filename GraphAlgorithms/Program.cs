using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Por favor, forneça o caminho do arquivo como argumento.");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Arquivo não encontrado.");
            return;
        }

     
        List<int> nosAcessados = new List<int>();
        Int64 totalGeral = 0;

        var graph = new Dictionary<int, List<(int, int)>>(); 
        int edgeCount = 0; 

       
        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith("a ")) 
            {
                var parts = line.Split();
                int from = int.Parse(parts[1]);
                int to = int.Parse(parts[2]);
                int weight = int.Parse(parts[3]);

                if (!graph.ContainsKey(from))
                    graph[from] = new List<(int, int)>();
                if (!graph.ContainsKey(to))
                    graph[to] = new List<(int, int)>();

                graph[from].Add((to, weight));
                edgeCount++; 
            }
        }

        int vertexCount = graph.Keys.Count; 
        var visitedNodes = new HashSet<int>();

        Console.WriteLine($"Grafo carregado com {vertexCount} vértices e {edgeCount} arestas.");

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        foreach (var node in graph.Keys)
        {
            if (!visitedNodes.Contains(node) )
            {
                Console.WriteLine($"Executando Dijkstra a partir do nó {node}...");
                var distances = Dijkstra(graph, node, out int totalCost);
                var ultimo = 0;
                foreach (var (destination, distance) in distances)
                {
                    if (distance != int.MaxValue) 
                    {
                        Console.WriteLine($"Menor distância de {node} para {destination}: {distance}");
                        if (!nosAcessados.Contains(destination))
                            nosAcessados.Add(destination);
                        ultimo = distance;
                    }
                    
                }

                Console.WriteLine($"Custo total das distâncias a partir do nó {node}: {totalCost}");
                visitedNodes.Add(node);
                totalGeral += ultimo;
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"");
        Console.WriteLine($"================================================================");
        Console.WriteLine($"Total de vértices : {vertexCount}");
        Console.WriteLine($"Total de arestas: {edgeCount} ");
        Console.WriteLine($"Distancia total : {totalGeral} ");
        Console.WriteLine($"Tempo total gasto: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("Processamento concluído.");
    }

    static Dictionary<int, int> Dijkstra(Dictionary<int, List<(int, int)>> graph, int startNode, out int totalCost)
    {
        var distances = new Dictionary<int, int>();
        var priorityQueue = new SortedSet<(int, int, int)>(Comparer<(int, int, int)>.Create((a, b) =>
        {
            if (a.Item1 != b.Item1) return a.Item1.CompareTo(b.Item1); 
            if (a.Item2 != b.Item2) return a.Item2.CompareTo(b.Item2); 
            return a.Item3.CompareTo(b.Item3); 
        }));

        int uniqueIdCounter = 0;
        totalCost = 0; 

       
        foreach (var node in graph.Keys)
        {
            distances[node] = int.MaxValue;
        }
        distances[startNode] = 0;
        priorityQueue.Add((0, startNode, uniqueIdCounter++));

        while (priorityQueue.Count > 0)
        {
            var (currentDistance, currentNode, _) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            if (currentDistance > distances[currentNode])
                continue;

            foreach (var (neighbor, weight) in graph[currentNode])
            {
                int newDistance = currentDistance + weight;
                if (newDistance < distances[neighbor])
                {
                    
                    priorityQueue.Remove((distances[neighbor], neighbor, 0));
                    distances[neighbor] = newDistance;

                  
                    priorityQueue.Add((newDistance, neighbor, uniqueIdCounter++));
                }
            }
        }

       
        foreach (var distance in distances.Values)
        {
            if (distance != int.MaxValue)
                totalCost += distance;
        }

        return distances;
    }
}
