using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OmniFinder;
using OmniFinder.Objects;
using OmniFinder.Objects.BuildingBlocks;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;

namespace GlycanTextToMass
{
    public static class Convert
    {
        [Test]
        private static void StringToMass()
        {
            string code = "5411";
            int charge = 2;

            double massToCharge;
            StringToMassFX(code, charge, out massToCharge);

            Assert.AreEqual(massToCharge, 1040.88785015808);
        }

        public static void StringToMassFX(string code, int charge, out double massToCharge)
        {
            int hex = System.Convert.ToInt32(code[0].ToString());
            int hexNAc = System.Convert.ToInt32(code[1].ToString());
            int fuc = System.Convert.ToInt32(code[2].ToString());
            int Neu5Ac = System.Convert.ToInt32(code[3].ToString());

            GlycanMakerObject newBasket = SetupBasket(hex, hexNAc, fuc, Neu5Ac);
            newBasket.Charge = charge;

            //inside
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(newBasket, newBasket.Charge);

            massToCharge = (double)results.MassToCharge;
            //Assert.AreEqual(results.MassNeutral, 2079.76114738262);
            //Assert.AreEqual(results.MassToCharge, 1040.88785015808);
        }



        private static GlycanMakerObject SetupBasket(int hex, int hexNAc, int fuc, int neu5Ac)
        {
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Alditol;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 2;
            newBasket.MassTollerance = 10;

            BuildingBlockMonoSaccharide exampleBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(hex));
            newBasket.LegoBuildingBlocks.Add(exampleBlock);

            BuildingBlockMonoSaccharide exampleBlock2 =
                new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(hexNAc));
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);

            BuildingBlockMonoSaccharide exampleBlock3 = new BuildingBlockMonoSaccharide(MonosaccharideName.Deoxyhexose, new RangesMinMax(fuc));
            newBasket.LegoBuildingBlocks.Add(exampleBlock3);

            BuildingBlockMonoSaccharide exampleBlock4 =
                new BuildingBlockMonoSaccharide(MonosaccharideName.NeuraminicAcid, new RangesMinMax(neu5Ac));
            newBasket.LegoBuildingBlocks.Add(exampleBlock4);

            PNNLOmics.Data.Constants.Libraries.UserUnitLibrary myLibrary =
                new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            myLibrary.SetLibrary(
                new UserUnit("randomMass", "RM1", 1000, UserUnitName.User01),
                new UserUnit("randomMass2", "RM2", 2000, UserUnitName.User02),
                new UserUnit("randomMass3", "RM3", 3000, UserUnitName.User03));

            Constants.SetUserUnitLibrary(myLibrary);
            BuildingBlockUserUnit exampleBlock5 = new BuildingBlockUserUnit(UserUnitName.User01, new RangesMinMax(0, 0));
            //exampleBlock3.BlockType = BuildingBlockType.UserUnit;
            newBasket.LegoBuildingBlocks.Add(exampleBlock5);

            newBasket.OmniFinderParameter.UserUnitLibrary = myLibrary;
            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }
    }
}
