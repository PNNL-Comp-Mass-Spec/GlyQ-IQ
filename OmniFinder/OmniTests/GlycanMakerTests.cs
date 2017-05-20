using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OmniFinder.Objects.Enumerations;
using OmniFinder.Objects;
using PNNLOmics.Data.Constants;
using OmniFinder;
using OmniFinder.Objects.BuildingBlocks;

namespace OmniTests
{
    public class GlycanMakerTests
    {
        [Test]
        public void GlycanMaker()
        {
            //create from outside and pass in
            GlycanMakerObject newBasket = CreateGlycanMakerObject();

            //inside
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(newBasket, newBasket.Charge);

            Assert.AreEqual(results.MassNeutral, 3368.840913848038);
            Assert.AreEqual(results.MassToCharge, 1685.427733390789);

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 90);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 149);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 66);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 6);

        }

        [Test]
        public void GlycanMakerFragment()
        {
            //create from outside and pass in
            GlycanMakerObject newBasket = CreateGlycanMakerObject();

            //this adds 2 na  for +2 ion
            newBasket.Charge = 1;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.CarbohydrateType = CarbType.Fragment;
            //inside
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(newBasket, newBasket.Charge);

            Assert.AreEqual(results.MassNeutral, 3368.840913848038);
            Assert.AreEqual(results.MassToCharge, 3369.848190314808m);

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 90);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 148);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 66);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 6);
            Assert.AreEqual(results.ResultComposition[ElementName.Sodium], 0);

        }

        [Test]
        public void GlycanMakerK()
        {
            //create from outside and pass in
            GlycanMakerObject newBasket = CreateGlycanMakerObject();

            //this adds 2 na  for +2 ion
            newBasket.Charge = 1;
            newBasket.ChargeCarryingAdduct = Adducts.K;
            newBasket.CarbohydrateType = CarbType.Alditol;
            //inside
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(newBasket, newBasket.Charge);

            Assert.AreEqual(results.MassNeutral, 3370.856563911958m);
            Assert.AreEqual(results.MassToCharge, 3409.81972226204857m);

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 90);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 150);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 66);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 6);
            Assert.AreEqual(results.ResultComposition[ElementName.Potassium], 1);

        }

        [Test]
        public void GlycanMakerMan5()
        {
            decimal massFromMZ; decimal massFromResults; int roundDecimal = 7;

            //create from outside and pass in
            GlycanMakerObject newBasket1 = CreateGlycanMakerObjectMan5();

            //this adds 2 na  for +2 ion
            newBasket1.Charge = 1;
            newBasket1.ChargeCarryingAdduct = Adducts.H;
            newBasket1.CarbohydrateType = CarbType.Alditol;
            //inside
            GlycanMakerOutput results1 = GlycanMakerController.CalculateMass(newBasket1, newBasket1.Charge);

            Assert.AreEqual(results1.MassNeutral, 1236.449076975336m);
            Assert.AreEqual(results1.MassToCharge, 1237.456353442106m);

            Assert.AreEqual(results1.ResultComposition[ElementName.Carbon], 46);
            Assert.AreEqual(results1.ResultComposition[ElementName.Hydrogen], 81);
            Assert.AreEqual(results1.ResultComposition[ElementName.Oxygen], 36);
            Assert.AreEqual(results1.ResultComposition[ElementName.Nitrogen], 2);
            Assert.AreEqual(results1.ResultComposition[ElementName.Sodium], 0);

            roundDecimal = 6;
            CalculateMass(results1, results1.MassToCharge, newBasket1.Charge, roundDecimal, out massFromMZ, out massFromResults);
            Assert.AreEqual(massFromResults, massFromMZ);

            //inside
            GlycanMakerObject newBasket2 = CreateGlycanMakerObjectMan5();
            newBasket2.Charge = 1;
            newBasket2.ChargeCarryingAdduct = Adducts.Na;
            newBasket2.CarbohydrateType = CarbType.Aldehyde;
            GlycanMakerOutput results2 = GlycanMakerController.CalculateMass(newBasket2, newBasket2.Charge);

            Assert.AreEqual(results2.MassNeutral, 1234.433426911416m);
            Assert.AreEqual(results2.MassToCharge, 1257.42264799410657m);

            Assert.AreEqual(results2.ResultComposition[ElementName.Carbon], 46);
            Assert.AreEqual(results2.ResultComposition[ElementName.Hydrogen], 78);
            Assert.AreEqual(results2.ResultComposition[ElementName.Oxygen], 36);
            Assert.AreEqual(results2.ResultComposition[ElementName.Nitrogen], 2);
            Assert.AreEqual(results2.ResultComposition[ElementName.Sodium], 1);

            roundDecimal = 7;
            CalculateMass(results2, results2.MassToCharge, newBasket2.Charge, roundDecimal, out massFromMZ, out massFromResults);
            Assert.AreEqual(massFromResults, massFromMZ);

            //inside
            GlycanMakerObject newBasket3 = CreateGlycanMakerObjectMan5();
            newBasket3.Charge = 1;
            newBasket3.ChargeCarryingAdduct = Adducts.DeProtonated;
            newBasket3.CarbohydrateType = CarbType.Alditol;
            GlycanMakerOutput results3 = GlycanMakerController.CalculateMass(newBasket3, newBasket3.Charge);

            Assert.AreEqual(results3.MassNeutral, 1236.449076975336m);
            Assert.AreEqual(results3.MassToCharge, 1235.441800508566m);

            Assert.AreEqual(results3.ResultComposition[ElementName.Carbon], 46);
            Assert.AreEqual(results3.ResultComposition[ElementName.Hydrogen], 79);
            Assert.AreEqual(results3.ResultComposition[ElementName.Oxygen], 36);
            Assert.AreEqual(results3.ResultComposition[ElementName.Nitrogen], 2);
            Assert.AreEqual(results3.ResultComposition[ElementName.Sodium], 0);

            roundDecimal = 2;
            CalculateMass(results3, results3.MassToCharge, newBasket3.Charge, roundDecimal, out massFromMZ, out massFromResults);
            Assert.AreEqual(massFromResults, massFromMZ);

            //inside
            GlycanMakerObject newBasket4 = CreateGlycanMakerObjectMan5();
            newBasket4.Charge = 1;
            newBasket4.ChargeCarryingAdduct = Adducts.Na;
            newBasket4.CarbohydrateType = CarbType.Fragment;
            GlycanMakerOutput results4 = GlycanMakerController.CalculateMass(newBasket4, newBasket4.Charge);

            Assert.AreEqual(results4.MassNeutral, 1216.422862225171m);
            Assert.AreEqual(results4.MassToCharge, 1239.41208330786157m);

            Assert.AreEqual(results4.ResultComposition[ElementName.Carbon], 46);
            Assert.AreEqual(results4.ResultComposition[ElementName.Hydrogen], 76);
            Assert.AreEqual(results4.ResultComposition[ElementName.Oxygen], 35);
            Assert.AreEqual(results4.ResultComposition[ElementName.Nitrogen], 2);
            Assert.AreEqual(results4.ResultComposition[ElementName.Sodium], 1);

            roundDecimal = 6;
            CalculateMass(results4, results4.MassToCharge, newBasket4.Charge, roundDecimal, out massFromMZ, out massFromResults);
            Assert.AreEqual(massFromResults, massFromMZ);
        }

        [Test]
        public void GlycanMakerNa()
        {
            //create from outside and pass in
            GlycanMakerObject newBasket = CreateGlycanMakerObject();

            //this adds 2 na  for +2 ion
            newBasket.Charge = 1;
            newBasket.ChargeCarryingAdduct = Adducts.Na;
            newBasket.CarbohydrateType = CarbType.Aldehyde;
            //inside
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(newBasket, newBasket.Charge);

            Assert.AreEqual(results.MassNeutral, 3368.840913848038);
            Assert.AreEqual(results.MassToCharge, 3391.83013493072857m);

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 90);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 148);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 66);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 6);
            Assert.AreEqual(results.ResultComposition[ElementName.Sodium], 1);

        }

        [Test]
        public void TestGlycanMakerObject()
        {
            GlycanMakerObject newBasket = CreateGlycanMakerObject();
            Assert.AreEqual(newBasket.CarbohydrateType, CarbType.Aldehyde);
            Assert.AreEqual(newBasket.ChargeCarryingAdduct, Adducts.H);
            Assert.AreEqual(newBasket.Charge, 2);
            Assert.AreEqual(newBasket.MassTollerance, 10);

            Assert.AreEqual(newBasket.LegoBuildingBlocks.Count, 5);
            Assert.AreEqual(newBasket.LegoBuildingBlocks[0].BlockType, BuildingBlockType.Monosaccharide);
            Assert.AreEqual(newBasket.LegoBuildingBlocks[1].Range.MaxRange, 4);
            Assert.AreEqual(newBasket.OmniFinderParameter.UserUnitLibrary[UserUnitName.User01].MassMonoIsotopic, 1000);
        }

        [Test]
        public void TestGlycanMakerProcessing()
        {
            GlycanMakerObject newBasket = CreateGlycanMakerObject();
            GlycanMakerObject updatedBasket = GlycanMakerController.PreProcessGlycanMakerObject(newBasket);

            Assert.AreEqual(newBasket.CarbohydrateType, CarbType.Aldehyde);
            Assert.AreEqual(newBasket.ChargeCarryingAdduct, Adducts.H);
            Assert.AreEqual(newBasket.Charge, 2);
            Assert.AreEqual(newBasket.MassTollerance, 10);

            Assert.AreEqual(updatedBasket.LegoBuildingBlocks.Count, 5);
            Assert.AreEqual(updatedBasket.LegoBuildingBlocks[0].BlockType, BuildingBlockType.Monosaccharide);
            Assert.AreEqual(updatedBasket.LegoBuildingBlocks[1].Range.MaxRange, 4);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.UserUnitLibrary[UserUnitName.User01].MassMonoIsotopic, 1000);
                           
            Assert.AreEqual(updatedBasket.OmniFinderParameter.BuildingBlocksMonosacchcarides.Count, 4);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.BuildingBlocksUserUnit.Count, 1);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.BuildingBlocksAminoAcids.Count, 0);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.UserUnitLibrary[UserUnitName.User02].MassMonoIsotopic, 2000);
        }


        public GlycanMakerObject CreateGlycanMakerObject()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Aldehyde;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 2;
            newBasket.MassTollerance = 10;

            BuildingBlockMonoSaccharide exampleBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(5));
            newBasket.LegoBuildingBlocks.Add(exampleBlock);

            BuildingBlockMonoSaccharide exampleBlock2 = new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(4));
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);

            BuildingBlockMonoSaccharide exampleBlock3 = new BuildingBlockMonoSaccharide(MonosaccharideName.Deoxyhexose, new RangesMinMax(1));
            newBasket.LegoBuildingBlocks.Add(exampleBlock3);

            BuildingBlockMonoSaccharide exampleBlock4 = new BuildingBlockMonoSaccharide(MonosaccharideName.NeuraminicAcid, new RangesMinMax(2));
            newBasket.LegoBuildingBlocks.Add(exampleBlock4);

            PNNLOmics.Data.Constants.Libraries.UserUnitLibrary myLibrary = new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            myLibrary.SetLibrary(
                new UserUnit("randomMass", "RM1", 1000, UserUnitName.User01),
                new UserUnit("randomMass2", "RM2", 2000, UserUnitName.User02),
                new UserUnit("randomMass3", "RM3", 3000, UserUnitName.User03));

            Constants.SetUserUnitLibrary(myLibrary);
            BuildingBlockUserUnit exampleBlock5 = new BuildingBlockUserUnit(UserUnitName.User01, new RangesMinMax(1, 1));
            //exampleBlock3.BlockType = BuildingBlockType.UserUnit;
            newBasket.LegoBuildingBlocks.Add(exampleBlock5);

            newBasket.OmniFinderParameter.UserUnitLibrary = myLibrary;
            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }

        public GlycanMakerObject CreateGlycanMakerObjectMan5()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Aldehyde;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 1;
            newBasket.MassTollerance = 10;

            BuildingBlockMonoSaccharide exampleBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(5));
            newBasket.LegoBuildingBlocks.Add(exampleBlock);

            BuildingBlockMonoSaccharide exampleBlock2 = new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2));
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);

            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }

        private static void CalculateMass(GlycanMakerOutput results, decimal MZ, int charge, int roundDecimal, out decimal massFromMZ, out decimal massFromResults)
        {
            double elementsMass =
                Constants.Elements[ElementName.Carbon].MassMonoIsotopic * results.ResultComposition[ElementName.Carbon] +
                Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic * results.ResultComposition[ElementName.Hydrogen] +
                Constants.Elements[ElementName.Oxygen].MassMonoIsotopic * results.ResultComposition[ElementName.Oxygen] +
                Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic * results.ResultComposition[ElementName.Nitrogen] +
                Constants.Elements[ElementName.Sodium].MassMonoIsotopic * results.ResultComposition[ElementName.Sodium];

            //make up for lost electron.  Deprotonation is slightly off in the check
            massFromMZ = results.MassToCharge * charge + (decimal)Constants.SubAtomicParticles[SubAtomicParticleName.Electron].MassMonoIsotopic*charge;

            massFromResults = (decimal)Math.Round(elementsMass, roundDecimal);
            massFromMZ = Math.Round(massFromMZ, roundDecimal);
        }
    }
}
