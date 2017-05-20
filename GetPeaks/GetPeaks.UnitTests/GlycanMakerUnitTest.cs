using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OmniFinder.Objects;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;
using GetPeaks_DLL.Objects;
using OmniFinder;
using OmniFinder.Objects.BuildingBlocks;

namespace GetPeaks.UnitTests
{
    class GlycanMakerUnitTest
    {
        public GlycanMakerObject CreateGlycanMakerObjectStrangeUser()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Aldehyde;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 1;
            newBasket.MassTollerance = 10;

            BuildingBlockMonoSaccharide exampleBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(3, 3));
            ///exampleBlock.BlockType = BuildingBlockType.Monosaccharide;
            newBasket.LegoBuildingBlocks.Add(exampleBlock);

            BuildingBlockAminoAcid exampleBlock2 = new BuildingBlockAminoAcid(AminoAcidName.Arginine, new RangesMinMax(3, 3));
            //exampleBlock2.BlockType = BuildingBlockType.AminoAcid;
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);

            PNNLOmics.Data.Constants.Libraries.UserUnitLibrary myLibrary = new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            UserUnit user01 = new UserUnit("randomMass1", "RM1", 1000, UserUnitName.User01);
            UserUnit user02 = new UserUnit("randomMass2", "RM2", 2000, UserUnitName.User02);
            UserUnit user03 = new UserUnit("randomMass2", "RM3", 3000, UserUnitName.User03);

            myLibrary.SetLibrary(user01, user02, user03);
            //myLibrary.SetLibrary(
            //    new UserUnit("randomMass", "RM1", 1000, UserUnitName.User01),
            //    new UserUnit("randomMass2", "RM2", 2000, UserUnitName.User02),
            //    new UserUnit("randomMass3", "RM3", 3000, UserUnitName.User03));

            Constants.SetUserUnitLibrary(myLibrary);
            BuildingBlockUserUnit exampleBlock3 = new BuildingBlockUserUnit(UserUnitName.User01, new RangesMinMax(3, 3));
            //exampleBlock3.BlockType = BuildingBlockType.UserUnit;
            newBasket.LegoBuildingBlocks.Add(exampleBlock3);

            newBasket.OmniFinderParameter.UserUnitLibrary = myLibrary;
            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }

        public GlycanMakerObject CreateGlycanMakerObjectMan5()
        {
            //hex5 hexnac2 Na1 Alditol
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Alditol;
            newBasket.ChargeCarryingAdduct = Adducts.Na;
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

        public GlycanMakerObject CreateGlycanMakerObjectGlycoPeptide()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Glycopeptide;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 2;
            newBasket.MassTollerance = 10;

            //Charles Nwosu
            //SGQNV HexNAc2 Hex8 2H  at 1103.91 or mono 2205.8153
            BuildingBlockMonoSaccharide exampleBlock1 = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(8));
            BuildingBlockMonoSaccharide exampleBlock2 = new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2));
            
            BuildingBlockAminoAcid exampleBlock3 = new BuildingBlockAminoAcid(AminoAcidName.Serine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock4 = new BuildingBlockAminoAcid(AminoAcidName.Glycine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock5 = new BuildingBlockAminoAcid(AminoAcidName.Glutamine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock6 = new BuildingBlockAminoAcid(AminoAcidName.Asparagine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock7 = new BuildingBlockAminoAcid(AminoAcidName.Valine, new RangesMinMax(1));

            newBasket.LegoBuildingBlocks.Add(exampleBlock1);
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);
            newBasket.LegoBuildingBlocks.Add(exampleBlock3);
            newBasket.LegoBuildingBlocks.Add(exampleBlock4);
            newBasket.LegoBuildingBlocks.Add(exampleBlock5);
            newBasket.LegoBuildingBlocks.Add(exampleBlock6);
            newBasket.LegoBuildingBlocks.Add(exampleBlock7);

            PNNLOmics.Data.Constants.Libraries.UserUnitLibrary myLibrary = new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            newBasket.OmniFinderParameter.UserUnitLibrary = myLibrary;
            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }

        public GlycanMakerObject CreateGlycanMakerObjectGlycoPeptide2()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Glycopeptide;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 2;
            newBasket.MassTollerance = 10;

            //Charles Nwosu
            //GEPTSTPT HexNAc1 Hex1 Neu5Ac1 2H  at 723.29 or mono 1444.58
            BuildingBlockMonoSaccharide exampleBlock1 = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(1));
            BuildingBlockMonoSaccharide exampleBlock2 = new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(1));
            BuildingBlockMonoSaccharide exampleBlock3 = new BuildingBlockMonoSaccharide(MonosaccharideName.NeuraminicAcid, new RangesMinMax(1));

            BuildingBlockAminoAcid exampleBlock4 = new BuildingBlockAminoAcid(AminoAcidName.Isoleucine, new RangesMinMax(2));
            BuildingBlockAminoAcid exampleBlock5 = new BuildingBlockAminoAcid(AminoAcidName.Proline, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock6 = new BuildingBlockAminoAcid(AminoAcidName.Threonine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock7 = new BuildingBlockAminoAcid(AminoAcidName.Asparagine, new RangesMinMax(1));

            BuildingBlockMiscellaneousMatter exampleBlock8 = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.Water, new RangesMinMax(1));
            
            newBasket.LegoBuildingBlocks.Add(exampleBlock1);
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);
            newBasket.LegoBuildingBlocks.Add(exampleBlock3);
            newBasket.LegoBuildingBlocks.Add(exampleBlock4);
            newBasket.LegoBuildingBlocks.Add(exampleBlock5);
            newBasket.LegoBuildingBlocks.Add(exampleBlock6);
            newBasket.LegoBuildingBlocks.Add(exampleBlock7);
            newBasket.LegoBuildingBlocks.Add(exampleBlock8);

            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }

        public GlycanMakerObject CreateGlycanMakerObjectGlycoPeptide3()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Glycopeptide;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 2;
            newBasket.MassTollerance = 10;

            //Charles Nwosu
            //GEPTSTPT HexNAc1 Hex1 Neu5Ac1 2H  at 723.29 or mono 1444.58
            BuildingBlockMonoSaccharide exampleBlock1 = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(5));
            BuildingBlockMonoSaccharide exampleBlock2 = new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2));

            BuildingBlockAminoAcid exampleBlock4 = new BuildingBlockAminoAcid(AminoAcidName.Serine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock5 = new BuildingBlockAminoAcid(AminoAcidName.Glycine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock6 = new BuildingBlockAminoAcid(AminoAcidName.Glutamine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock7 = new BuildingBlockAminoAcid(AminoAcidName.Asparagine, new RangesMinMax(1));


            newBasket.LegoBuildingBlocks.Add(exampleBlock1);
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);
            //newBasket.LegoBuildingBlocks.Add(exampleBlock3);
            newBasket.LegoBuildingBlocks.Add(exampleBlock4);
            newBasket.LegoBuildingBlocks.Add(exampleBlock5);
            newBasket.LegoBuildingBlocks.Add(exampleBlock6);
            newBasket.LegoBuildingBlocks.Add(exampleBlock7);


            PNNLOmics.Data.Constants.Libraries.UserUnitLibrary myLibrary = new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            newBasket.OmniFinderParameter.UserUnitLibrary = myLibrary;
            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }

        public GlycanMakerObject CreateGlycanMakerObjectGlycoPeptide4()
        {
            //inputs
            GlycanMakerObject newBasket = new GlycanMakerObject();
            newBasket.CarbohydrateType = CarbType.Glycopeptide;
            newBasket.ChargeCarryingAdduct = Adducts.H;
            newBasket.Charge = 2;
            newBasket.MassTollerance = 10;

            //Charles Nwosu
            //GEPTSTPT HexNAc1 Hex1 Neu5Ac1 2H  at 723.29 or mono 1444.58
            BuildingBlockMonoSaccharide exampleBlock1 = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(5));
            BuildingBlockMonoSaccharide exampleBlock2 = new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2));

            BuildingBlockAminoAcid exampleBlock4 = new BuildingBlockAminoAcid(AminoAcidName.Serine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock5 = new BuildingBlockAminoAcid(AminoAcidName.Glycine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock6 = new BuildingBlockAminoAcid(AminoAcidName.Glutamine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock7 = new BuildingBlockAminoAcid(AminoAcidName.Asparagine, new RangesMinMax(1));
            BuildingBlockAminoAcid exampleBlock8 = new BuildingBlockAminoAcid(AminoAcidName.Valine, new RangesMinMax(1));


            newBasket.LegoBuildingBlocks.Add(exampleBlock1);
            newBasket.LegoBuildingBlocks.Add(exampleBlock2);
            //newBasket.LegoBuildingBlocks.Add(exampleBlock3);
            newBasket.LegoBuildingBlocks.Add(exampleBlock4);
            newBasket.LegoBuildingBlocks.Add(exampleBlock5);
            newBasket.LegoBuildingBlocks.Add(exampleBlock6);
            newBasket.LegoBuildingBlocks.Add(exampleBlock7);
            newBasket.LegoBuildingBlocks.Add(exampleBlock8);

            PNNLOmics.Data.Constants.Libraries.UserUnitLibrary myLibrary = new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            newBasket.OmniFinderParameter.UserUnitLibrary = myLibrary;
            newBasket.OmniFinderParameter.CarbohydrateType = newBasket.CarbohydrateType;
            newBasket.OmniFinderParameter.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;

            return newBasket;
        }


        [Test]
        public void TestGlycanMakerObject()//passes
        {
            GlycanMakerObject newBasket = CreateGlycanMakerObjectStrangeUser();
            Assert.AreEqual(newBasket.CarbohydrateType, CarbType.Aldehyde);
            Assert.AreEqual(newBasket.ChargeCarryingAdduct, Adducts.H);
            Assert.AreEqual(newBasket.Charge, 1);
            Assert.AreEqual(newBasket.MassTollerance, 10);

            Assert.AreEqual(newBasket.LegoBuildingBlocks.Count, 3);
            Assert.AreEqual(newBasket.LegoBuildingBlocks[0].BlockType, BuildingBlockType.Monosaccharide);
            Assert.AreEqual(newBasket.LegoBuildingBlocks[1].Range.MaxRange, 3);
            Assert.AreEqual(newBasket.OmniFinderParameter.UserUnitLibrary[UserUnitName.User01].MassMonoIsotopic, 1000);
        }

        [Test]
        public void TestGlycanMakerProcessing()//passes
        {
            GlycanMakerObject newBasket = CreateGlycanMakerObjectStrangeUser();
            GlycanMakerObject updatedBasket = GlycanMakerController.PreProcessGlycanMakerObject(newBasket);

            Assert.AreEqual(newBasket.CarbohydrateType, CarbType.Aldehyde);
            Assert.AreEqual(newBasket.ChargeCarryingAdduct, Adducts.H);
            Assert.AreEqual(newBasket.Charge, 1);
            Assert.AreEqual(newBasket.MassTollerance, 10);

            Assert.AreEqual(updatedBasket.LegoBuildingBlocks.Count, 3);
            Assert.AreEqual(updatedBasket.LegoBuildingBlocks[0].BlockType, BuildingBlockType.Monosaccharide);
            Assert.AreEqual(updatedBasket.LegoBuildingBlocks[1].Range.MaxRange, 3);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.UserUnitLibrary[UserUnitName.User01].MassMonoIsotopic, 1000);
                           
            Assert.AreEqual(updatedBasket.OmniFinderParameter.BuildingBlocksMonosacchcarides.Count, 1);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.BuildingBlocksUserUnit.Count, 1);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.BuildingBlocksAminoAcids.Count, 1);
            Assert.AreEqual(updatedBasket.OmniFinderParameter.UserUnitLibrary[UserUnitName.User02].MassMonoIsotopic, 2000);
        }

        [Test]
        public void GlycanMakerTest()
        {
            GlycanMakerObject inputForGlycanMaker = CreateGlycanMakerObjectStrangeUser();
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            Assert.AreEqual(results.MassNeutral, 3972.472368086471);//if this is 18.01056468245 then the range is not 1-1 etc

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 24);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 45);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 17);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 4);

            double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
            double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
            double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
            double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;

            double mass =
                results.ResultComposition[ElementName.Carbon] * massCarbon +
                results.ResultComposition[ElementName.Hydrogen] * massHydrogen +
                results.ResultComposition[ElementName.Nitrogen] * massNitrogen +
                results.ResultComposition[ElementName.Oxygen] * massOxygen;

            Assert.AreEqual(mass+3000, results.MassNeutral);
        }

        [Test]
        public void GlycanMakerTestMan5()//passes
        {
            GlycanMakerObject inputForGlycanMaker = CreateGlycanMakerObjectMan5();
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            Assert.AreEqual(results.MassNeutral, 1236.449076975336m);//if this is 18.01056468245 then the range is not 1-1 etc
            Assert.AreEqual(results.MassToCharge, 1259.43829805802657m);//if this is 18.01056468245 then the range is not 1-1 etc

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 46);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 80);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 36);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 2);
            Assert.AreEqual(results.ResultComposition[ElementName.Sodium], 1);

            double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
            double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
            double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
            double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;

            double mass =
                results.ResultComposition[ElementName.Carbon] * massCarbon +
                results.ResultComposition[ElementName.Hydrogen] * massHydrogen +
                results.ResultComposition[ElementName.Nitrogen] * massNitrogen +
                results.ResultComposition[ElementName.Oxygen] * massOxygen;

            Assert.AreEqual(mass, results.MassNeutral);
        }

        [Test]
        public void GlycopeptideMakerTest()//passes
        {
            GlycanMakerObject inputForGlycanMaker = CreateGlycanMakerObjectGlycoPeptide();
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            Assert.AreEqual(results.MassNeutral, 2205.815308226377m);//if this is 18.01056468245 then the range is not 1-1 etc
            Assert.AreEqual(results.MassToCharge, 1103.9149305799585m);//if this is 18.01056468245 then the range is not 1-1 etc

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 83);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 139);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 59);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 9);

            double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
            double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
            double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
            double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;

            double mass =
                results.ResultComposition[ElementName.Carbon] * massCarbon +
                results.ResultComposition[ElementName.Hydrogen] * massHydrogen +
                results.ResultComposition[ElementName.Nitrogen] * massNitrogen +
                results.ResultComposition[ElementName.Oxygen] * massOxygen;

            Assert.AreEqual(mass, results.MassNeutral);
        }

        [Test]
        public void GlycopeptideMakerTest2()
        {
            GlycanMakerObject inputForGlycanMaker = CreateGlycanMakerObjectGlycoPeptide2();
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            Assert.AreEqual(results.MassNeutral, 1212.549674924434m);//if this is 18.01056468245 then the range is not 1-1 etc
            Assert.AreEqual(results.MassToCharge, 607.282113928987m);//if this is 18.01056468245 then the range is not 1-1 etc

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 44);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 73);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 25);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 7);

            double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
            double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
            double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
            double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;

            double mass =
                results.ResultComposition[ElementName.Carbon] * massCarbon +
                results.ResultComposition[ElementName.Hydrogen] * massHydrogen +
                results.ResultComposition[ElementName.Nitrogen] * massNitrogen +
                results.ResultComposition[ElementName.Oxygen] * massOxygen;

            Assert.AreEqual(mass, results.MassNeutral);
        }

        [Test]
        public void GlycopeptideMakerTest3()//passes
        {
            GlycanMakerObject inputForGlycanMaker = CreateGlycanMakerObjectGlycoPeptide3();
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            Assert.AreEqual(results.MassNeutral, 1620.588424015319m);//if this is 18.01056468245 then the range is not 1-1 etc
            Assert.AreEqual(results.MassToCharge, 811.3014884744295m);//if this is 18.01056468245 then the range is not 1-1 etc

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 60);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 100);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 43);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 8);

            double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
            double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
            double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
            double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;

            double mass =
                results.ResultComposition[ElementName.Carbon] * massCarbon +
                results.ResultComposition[ElementName.Hydrogen] * massHydrogen +
                results.ResultComposition[ElementName.Nitrogen] * massNitrogen +
                results.ResultComposition[ElementName.Oxygen] * massOxygen;

            Assert.AreEqual(mass, results.MassNeutral);
        }

        [Test]
        public void GlycopeptideMakerTest4()//passes
        {
            GlycanMakerObject inputForGlycanMaker = CreateGlycanMakerObjectGlycoPeptide4();
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            Assert.AreEqual(results.MassNeutral, 1719.656837932702d);//if this is 18.01056468245 then the range is not 1-1 etc
            Assert.AreEqual(results.MassToCharge, 860.835695433121m);//if this is 18.01056468245 then the range is not 1-1 etc

            Assert.AreEqual(results.ResultComposition[ElementName.Carbon], 65);
            Assert.AreEqual(results.ResultComposition[ElementName.Hydrogen], 109);
            Assert.AreEqual(results.ResultComposition[ElementName.Oxygen], 44);
            Assert.AreEqual(results.ResultComposition[ElementName.Nitrogen], 9);

            double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
            double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
            double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
            double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;

            double mass =
                results.ResultComposition[ElementName.Carbon] * massCarbon +
                results.ResultComposition[ElementName.Hydrogen] * massHydrogen +
                results.ResultComposition[ElementName.Nitrogen] * massNitrogen +
                results.ResultComposition[ElementName.Oxygen] * massOxygen;

            Assert.AreEqual(Math.Round(mass,12), Math.Round(results.MassNeutral,12));
        }
    }
}
