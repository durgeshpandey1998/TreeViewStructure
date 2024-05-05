using Microsoft.AspNetCore.Mvc;
using PracticaParentChildStructure.Models;
using PracticaParentChildStructure.MyDBContext;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

namespace PracticaParentChildStructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<NodeViewModel> nodes = new List<NodeViewModel>();
            var nodedata = DatabaseService.GetValues();

            foreach (var data in nodedata)
            {
                AddNode(data, nodes);
            }

            ViewBag.Json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
            return View();
        }
        [HttpPost]
        public IActionResult Index(string selectedItems)
        {

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult DeleteNode(string selectedData)
        {
            DatabaseService.DeleteNode(Convert.ToInt32(selectedData));
            return RedirectToAction("Index", "Home");
        }

        //Recursion Function 
        private void AddNode(Node node, List<NodeViewModel> nodes)
        {
            if (node.IsActive == 1)
            {
                nodes.Add(new NodeViewModel
                {
                    id = node.NodeId.ToString(),
                    parent = node.ParentNodeId.ToString(),
                    text = node.NodeName
                });

                if (node.Children != null)
                {
                    foreach (var child in node.Children)
                    {
                        if (child.IsActive==1)
                        {
                            AddNode(child, nodes);
                        }
                    }
                }
            }
        }

        public IActionResult CreateNode()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateNode(Node node)
        {
            node.StartDate = DateTime.Now;
            DatabaseService.SaveValue(node);
            return RedirectToAction("Index", "Home");
        }
        public JsonResult BindAllNode()
        {
            var data = DatabaseService.GetNodeToBind();
            var binddata =  JsonSerializer.Serialize(data);
           return Json(binddata);
        }

        //List<NodeViewModel> nodes = new List<NodeViewModel>();
        //Node node = new Node();
        //var nodedata = GetStaticNodeList();
        //foreach (Node data in nodedata)
        //{
        //    nodes.Add(new NodeViewModel
        //    {
        //        id = data.NodeId.ToString(),
        //        parent = "#",
        //        text = data.NodeName
        //    });
        //}
        //foreach (Node data in nodedata)
        //{
        //    foreach (Node item in data.Children)
        //    {
        //        nodes.Add(new NodeViewModel
        //        {
        //            id = item.ParentNodeId.ToString() + "-" + item.NodeId.ToString(),
        //            parent = item.ParentNodeId.ToString(),
        //            text = item.NodeName
        //        });
        //    }
        //}
        ////ViewBag.Json = JsonSerializer.Serialize(nodes);
        //ViewBag.Json = nodes;
        //return View();

        // }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public static List<Node> GetStaticNodeList()
        //{
        //    var nodeList = new List<Node>();

        //    // Create some sample nodes
        //    var node1 = new Node
        //    {
        //        NodeId = 1,
        //        NodeName = "Node 1",
        //        Children = new List<Node>
        //    {
        //        new Node { NodeId = 2, NodeName = "Node 1.1", Children = null, ParentNodeId = 1 },
        //        new Node { NodeId = 3, NodeName = "Node 1.2", Children = null, ParentNodeId = 1 }
        //    }
        //    };

        //    var node2 = new Node
        //    {
        //        NodeId = 4,
        //        NodeName = "Node 2",
        //        Children = new List<Node>
        //    {
        //        new Node { NodeId = 5, NodeName = "Node 2.1", Children = null, ParentNodeId = 4 }
        //    }
        //    };
        //    var node3 = new Node
        //    {
        //        NodeId = 6,
        //        NodeName = "Node 3",
        //        Children = new List<Node>
        //    {
        //        new Node { NodeId = 7, NodeName = "Node 3.1", Children = null, ParentNodeId = 6 }
        //    }
        //    };

        //    nodeList.Add(node1);
        //    nodeList.Add(node2);
        //    nodeList.Add(node3);

        //    return nodeList;
        //}

    }
}