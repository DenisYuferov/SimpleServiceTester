using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;

namespace SimpleServiceTester.Models
{
    /// <summary>
    /// Service parcer
    /// </summary>
    public class ServiceParser
    {
        private readonly Uri _uri;

        public ServiceParser(string serviceAddress)
        {
            _uri = new Uri(serviceAddress + "?wsdl");
        }

        public List<Type> GetTypes()
        {
            var contractsDescriptions = GetContractsDescriptions();
            var generator = GetContractsGenerator(contractsDescriptions);
            var types = GetTypesFromGenerator(generator);

            return types.ToList();
        }

        private IEnumerable<ContractDescription> GetContractsDescriptions()
        {
            var mexClient = new MetadataExchangeClient(_uri, MetadataExchangeClientMode.HttpGet)
            {
                ResolveMetadataReferences = true
            };

            var metaDataSet = mexClient.GetMetadata();

            var wdslImporter = new WsdlImporter(metaDataSet);

            return wdslImporter.ImportAllContracts();
        }

        private ServiceContractGenerator GetContractsGenerator(IEnumerable<ContractDescription> contractDescriptions)
        {
            var generator = new ServiceContractGenerator();

            foreach (ContractDescription contract in contractDescriptions)
            {
                generator.GenerateServiceContractType(contract);
            }

            return generator;
        }

        private Type[] GetTypesFromGenerator(ServiceContractGenerator generator)
        {
            var codeDomProvider = CodeDomProvider.CreateProvider("C#");

            var assemlyNames = new[] { "System.dll", "System.ServiceModel.dll", "System.Runtime.Serialization.dll" };

            var compilerParameters = new CompilerParameters(assemlyNames) { GenerateInMemory = true };

            var compilerResults = codeDomProvider.CompileAssemblyFromDom(compilerParameters, generator.TargetCompileUnit);

            return compilerResults.CompiledAssembly.GetTypes();
        }
    }
}
