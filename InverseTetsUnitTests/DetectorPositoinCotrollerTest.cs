using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InverseTest.Frame;

namespace InverseTetsUnitTests
{
    [TestClass]
    public class DetectorPositionControllerTest
    {

        [TestMethod]
        public void DetectorPositionController_Global()
        {
            var x = DetectorPositionController.XGlobalToLocal(50);
            Assert.AreEqual(-500, x);

            var y = DetectorPositionController.YGlobalToLocal(50);
            Assert.AreEqual(500, y);
            
            var z = DetectorPositionController.ZGlobalToLocal(50);
            Assert.AreEqual(-120, z);            


            var a = DetectorPositionController.AGlobalToLocal(50);
            Assert.AreEqual(-2778 ,Math.Round(a));
            var b = DetectorPositionController.BGlobalToLocal(50);
            Assert.AreEqual(-2825,  Math.Round(b));
        }


        [TestMethod]
        public void ManipulatorPosition_GlobalToLocal()
        {

            var XGlobalIn = 140;
            var XLocal = DetectorPositionController.XGlobalToLocal(XGlobalIn);
            var XGlobal = DetectorPositionController.XLocalToGlobal(XLocal);
            Assert.AreEqual(XGlobalIn, XGlobal);

            var YGlobalIn = 140;
            var YLocal = DetectorPositionController.YGlobalToLocal(YGlobalIn);
            var TGlobal = DetectorPositionController.YLocalToGlobal(YLocal);
            Assert.AreEqual(YGlobalIn, TGlobal);

            var ZGlobalIn = 140;
            var ZLocal = DetectorPositionController.ZGlobalToLocal(ZGlobalIn);
            var ZGlobal = DetectorPositionController.ZLocalToGlobal(ZLocal);
            Assert.AreEqual(ZGlobalIn, ZGlobal);

            var AGlobalIn = 140;
            var ALocal = DetectorPositionController.AGlobalToLocal(AGlobalIn);
            var AGlobal = DetectorPositionController.ALocalToGlobal(ALocal);
            Assert.AreEqual(AGlobalIn, AGlobal);

            var BGlobalIn = 140;
            var BLocal = DetectorPositionController.BGlobalToLocal(BGlobalIn);
            var BGlobal = DetectorPositionController.BLocalToGlobal(BLocal);
            Assert.AreEqual(BGlobalIn, AGlobal);
        }
    }
}
