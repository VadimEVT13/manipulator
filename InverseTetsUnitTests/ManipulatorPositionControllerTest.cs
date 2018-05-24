using InverseTest.Manipulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InverseTestUnitTest
{

    [TestClass]
    public class ManipulatorPositionControllerTest
    {

        [TestMethod]
        public void ManipulatorPosition_Global()
        {
            var t1 = ManipulatorPositionController.T1GlobalToLocal(50);
            Assert.AreEqual(140, t1);

            var t2 = ManipulatorPositionController.T2GlobalToLocal(50);
            Assert.AreEqual(40, t2);

            var t3 = ManipulatorPositionController.T3GlobalToLocal(50);
            Assert.AreEqual(140, t3);

            var t4 = ManipulatorPositionController.T4GlobalToLocal(50);
            Assert.AreEqual(50, t4);

            var t5 = ManipulatorPositionController.T5GlobalToLocal(50);
            Assert.AreEqual(140, t5);
        }


        [TestMethod]
        public void ManipulatorPosition_GlobalToLocal()
        {

            var T1GlobalIn = 140;
            var t1Local = ManipulatorPositionController.T1GlobalToLocal(T1GlobalIn);
            var t1Global = ManipulatorPositionController.T1LocalToGlobal(t1Local);
            Assert.AreEqual(T1GlobalIn, t1Global);

            var T2GlobalIn = 140;
            var t2Local = ManipulatorPositionController.T2GlobalToLocal(T2GlobalIn);
            var t2Global = ManipulatorPositionController.T2LocalToGlobal(t2Local);
            Assert.AreEqual(T2GlobalIn, t2Global);

            var T3GlobalIn = 140;
            var t3Local = ManipulatorPositionController.T3GlobalToLocal(T3GlobalIn);
            var t3Global = ManipulatorPositionController.T3LocalToGlobal(t3Local);
            Assert.AreEqual(T3GlobalIn, t3Global);

            var T4GlobalIn = 140;
            var t4Local = ManipulatorPositionController.T4GlobalToLocal(T4GlobalIn);
            var t4Global = ManipulatorPositionController.T4LocalToGlobal(t4Local);
            Assert.AreEqual(T4GlobalIn, t4Global);

            var T5GlobalIn = 140;
            var t5Local = ManipulatorPositionController.T5GlobalToLocal(T5GlobalIn);
            var t5Global = ManipulatorPositionController.T5LocalToGlobal(t5Local);
            Assert.AreEqual(T5GlobalIn, t5Global);
        }
    }
}
