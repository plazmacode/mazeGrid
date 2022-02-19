using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGrid
{
    public class Edge<T>
    {
        private Node<T> from;
        private Node<T> to;
        public Node<T> To { get => to; set => to = value; }
        public Node<T> From { get => from; set => from = value; }

        public Edge(Node<T> from, Node<T> to)
        {
            this.From = from;
            this.To = to;
        }

    }
}
