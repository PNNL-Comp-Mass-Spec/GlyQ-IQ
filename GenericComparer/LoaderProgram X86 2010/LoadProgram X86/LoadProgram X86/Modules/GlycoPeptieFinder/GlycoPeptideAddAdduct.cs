using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    interface IAddAdduct
    {
        void ToGlycanResidules(int siteNumber, double adductMass, peptidePosibilitiesPerSite peptideStorage);
        void ToPeptide(int siteNumber, double adductMass, peptidePosibilitiesPerSite peptideStorage);
        void ToCoreAdd(double adductMass, GlycoPeptideParameters glycoPeptideParameter);//add and remove parts of the core to filter glycan residules
        void ToCoreRemove(double adductMass, GlycoPeptideParameters glycoPeptideParameter);
    }
    
    class GlycoPeptideAddAdduct:  IAddAdduct
    {
        void IAddAdduct.ToPeptide(int siteNumber, double adductMass, peptidePosibilitiesPerSite peptideStorage)
        {
            List<double> PeptideMasses = peptideStorage.PeptideMassList[siteNumber];

            for (int i = 0; i < PeptideMasses.Count; i++)
            {
                PeptideMasses[i] += adductMass;
            }
        }
        void IAddAdduct.ToGlycanResidules(int siteNumber, double adductMass, peptidePosibilitiesPerSite peptideStorage)
        {
            List<double> GlycanMasses = peptideStorage.glycanResidulesMass[siteNumber];

            for (int i = 0; i < GlycanMasses.Count; i++)
            {
                GlycanMasses[i] += adductMass;
            }
        }

        void IAddAdduct.ToCoreAdd(double adductMass, GlycoPeptideParameters glycoPeptideParameter)
        {
            glycoPeptideParameter.CoreMass +=adductMass;
        }
        void IAddAdduct.ToCoreRemove(double adductMass, GlycoPeptideParameters glycoPeptideParameter)
        {
            glycoPeptideParameter.CoreMass -= adductMass;
        }
    }
}
