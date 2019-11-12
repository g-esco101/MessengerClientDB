using MessengerClientDB.Models;
using MessengerClientDB.Unit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MessengerClient.UnitTest
{
    [TestClass]
    public class TestRepository
    {


        [TestMethod]
        public void GetAllMessagesOrdered_DuplicateDateTime_125()
        {
            // Arrange
            Messages m2 = new Messages();
            m2.ID = 101;
            m2.SenderID = "Demo106";
            m2.ReceiverID = "Demo1";
            m2.Date = Convert.ToDateTime("10/13/2019 12:00:00 AM");
            m2.Time = TimeSpan.Parse("19:49:12");
            m2.Contents = "See you soon!";
            m2.Read = "Unread";

            Messages m1 = new Messages();
            m1.ID = 1006;
            m1.SenderID = "Demo106";
            m1.ReceiverID = "Demo100";
            m1.Date = Convert.ToDateTime("10/13/2019 12:00:00 AM");
            m1.Time = TimeSpan.Parse("20:49:12");
            m1.Contents = "Bonjour from the Grand!";
            m1.Read = "Unread";

            Messages m5 = new Messages();
            m5.ID = 101;
            m5.SenderID = "Demo106";
            m5.ReceiverID = "Demo1";
            m5.Date = Convert.ToDateTime("10/10/2019 12:00:00 AM");
            m5.Time = TimeSpan.Parse("19:49:12");
            m5.Contents = "OMW!";
            m5.Read = "Unread";

            Messages m9 = new Messages();
            m9.ID = 101;
            m9.SenderID = "Demo505";
            m9.ReceiverID = "Demo100";
            m9.Date = Convert.ToDateTime("09/20/2019 12:00:00 AM");
            m9.Time = TimeSpan.Parse("19:49:10");
            m9.Contents = "Wrong person...";
            m9.Read = "Unread";

            var data = new List<Messages>() { m2, m9, m1, m5 }.AsQueryable();

            var mockSet = new Mock<DbSet<Messages>>();
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<MessengerClient_DBEntities>();
            mockContext.Setup(c => c.Messages).Returns(mockSet.Object);

            // Act 
            var unitWork = new UnitOfWork(mockContext.Object);
            var messages = unitWork.messagesRepo.GetAllOrdered("Demo106").ToList();

            // Assert
            Assert.AreEqual(3, messages.Count());
            Assert.AreEqual(m1, messages[0]);
            Assert.AreEqual(m2, messages[1]);
            Assert.AreEqual(m5, messages[2]);
        }

        [TestMethod]
        public void GetAllMessagesOrdered_EmptyDb_0()
        {
            // Arrange
            var data = new List<Messages>().AsQueryable();

            var mockSet = new Mock<DbSet<Messages>>();
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<MessengerClient_DBEntities>();
            mockContext.Setup(c => c.Messages).Returns(mockSet.Object);

            // Act 
            var unitWork = new UnitOfWork(mockContext.Object);
            var messages = unitWork.messagesRepo.GetAllOrdered("Demo106").ToList();

            // Assert
            Assert.AreEqual(0, messages.Count());
        }

        [TestMethod]
        public void GetAllMessagesOrdered_NoMatches_0()
        {
            // Arrange
            Messages m5 = new Messages();
            m5.ID = 101;
            m5.SenderID = "Demo102";
            m5.ReceiverID = "Demo1";
            m5.Date = Convert.ToDateTime("10/10/2019 12:00:00 AM");
            m5.Time = TimeSpan.Parse("19:49:12");
            m5.Contents = "OMW!";
            m5.Read = "Unread";

            Messages m9 = new Messages();
            m9.ID = 101;
            m9.SenderID = "Demo505";
            m9.ReceiverID = "Demo100";
            m9.Date = Convert.ToDateTime("09/20/2019 12:00:00 AM");
            m9.Time = TimeSpan.Parse("19:49:10");
            m9.Contents = "Wrong person...";
            m9.Read = "Unread";

            var data = new List<Messages>() { m5, m9 }.AsQueryable();

            var mockSet = new Mock<DbSet<Messages>>();
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<MessengerClient_DBEntities>();
            mockContext.Setup(c => c.Messages).Returns(mockSet.Object);

            // Act 
            var unitWork = new UnitOfWork(mockContext.Object);
            var messages = unitWork.messagesRepo.GetAllOrdered("Demo106").ToList();

            // Assert
            Assert.AreEqual(0, messages.Count());
        }

        [TestMethod]
        public void RemoveRange_GiveContext4Remove2_2()
        {
            // Arrange
            Messages m2 = new Messages();
            m2.ID = 101;
            m2.SenderID = "Demo106";
            m2.ReceiverID = "Demo1";
            m2.Date = Convert.ToDateTime("10/13/2019 12:00:00 AM");
            m2.Time = TimeSpan.Parse("19:49:12");
            m2.Contents = "See you soon!";
            m2.Read = "Unread";

            Messages m1 = new Messages();
            m1.ID = 1006;
            m1.SenderID = "Demo106";
            m1.ReceiverID = "Demo100";
            m1.Date = Convert.ToDateTime("10/13/2019 12:00:00 AM");
            m1.Time = TimeSpan.Parse("20:49:12");
            m1.Contents = "Bonjour from the Grand!";
            m1.Read = "Unread";

            Messages m5 = new Messages();
            m5.ID = 101;
            m5.SenderID = "Demo106";
            m5.ReceiverID = "Demo1";
            m5.Date = Convert.ToDateTime("10/10/2019 12:00:00 AM");
            m5.Time = TimeSpan.Parse("19:49:12");
            m5.Contents = "OMW!";
            m5.Read = "Unread";

            Messages m9 = new Messages();
            m9.ID = 101;
            m9.SenderID = "Demo505";
            m9.ReceiverID = "Demo100";
            m9.Date = Convert.ToDateTime("09/20/2019 12:00:00 AM");
            m9.Time = TimeSpan.Parse("19:49:10");
            m9.Contents = "Wrong person...";
            m9.Read = "Unread";

            var data = new List<Messages>() { m2, m9, m1, m5 }.AsQueryable();

            var mockSet = new Mock<DbSet<Messages>>();
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<MessengerClient_DBEntities>();
            mockContext.Setup(c => c.Messages).Returns(mockSet.Object);
            var remove = new List<Messages>() { m2, m5 };

            // Act 
            var unitWork = new UnitOfWork(mockContext.Object);
            unitWork.messagesRepo.RemoveRange(remove);
            List<Messages> remaining = new List<Messages>() { m9, m1 };
            ;
            // Assert
            Assert.AreEqual(2, remove.Count());
            Assert.IsTrue(remaining.Contains(m1));
            Assert.IsTrue(remaining.Contains(m1));
        }

        [TestMethod]
        public void RemoveRange_GiveContext4Remove2_2()
        {
            // Arrange
            Messages m2 = new Messages();
            m2.ID = 101;
            m2.SenderID = "D";
            m2.ReceiverID = "Demo1";
            m2.Date = Convert.ToDateTime("10/13/2019 12:00:00 AM");
            m2.Time = TimeSpan.Parse("19:49:12");
            m2.Contents = "See you soon!";
            m2.Read = "Unread";


            var mockSet = new Mock<DbSet<Messages>>();
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Messages>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<MessengerClient_DBEntities>();
            mockContext.Setup(c => c.Messages).Returns(mockSet.Object);
            var remove = new List<Messages>() { m2, m5 };

            // Act 
            var unitWork = new UnitOfWork(mockContext.Object);
            unitWork.messagesRepo.RemoveRange(remove);
            List<Messages> remaining = new List<Messages>() { m9, m1 };
            ;
            // Assert
            Assert.AreEqual(2, remove.Count());
            Assert.IsTrue(remaining.Contains(m1));
            Assert.IsTrue(remaining.Contains(m1));
        }
    }
}
