using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGrid
{
    public class Node<T>
    {
        private T data;
        private bool discovered;
        private List<Edge<T>> edges = new List<Edge<T>>();
        public Node<T> Parent { get; set; }
        public List<Edge<T>> Edges { get => edges; set => edges = value; }
        public bool Discovered { get => discovered; set => discovered = value; }
        public T Data { get => data; set => data = value; }

        public Node(T data)
        {
            this.Data = data;
            Discovered = false;
        }

        public void AddEdge(Node<T> other)
        {
            Edges.Add(new Edge<T>(this, other));
        }
    }
}
