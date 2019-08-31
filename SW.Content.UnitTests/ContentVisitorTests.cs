using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class ContentVisitorTests
    {

        [TestMethod]
        public void TestVisitor()
        {
            var container = new ContainerDto
            {
                Attachments = new AttachmentDto[]
                {
                    new AttachmentDto{},
                    new AttachmentDto{}
                }
            };

            var contentNode = ContentFactory.Default.CreateFrom(container);

            var pairs = contentNode.Visit();

            
        }
    }
}
