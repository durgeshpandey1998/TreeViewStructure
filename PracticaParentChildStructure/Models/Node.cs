namespace PracticaParentChildStructure.Models
{
    
        public class Node
        {
            public int NodeId { get; set; }
            public string NodeName { get; set; }
            public string? ParentNodeId { get; set; }
            public int IsActive { get; set; }
            public DateTime StartDate { get; set; }
            public List<Node> Children { get; set; }
        }
    public class NodeViewModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
    }



}
