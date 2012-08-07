//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;

namespace Rooko.Core
{
	public interface IRepositoryFactory
	{
		ITableRepository CreateTableRepository();
		
		IMigrationRepository CreateSchemaRepository();
	}
	
	public class RookoContext
	{
		static IRepositoryFactory repositoryFactory;
		
		public RookoContext()
		{
		}
		
		public static void SetRepositoryFactory(IRepositoryFactory repositoryFactory)
		{
			RookoContext.repositoryFactory = repositoryFactory;
		}
		
		public static IRepositoryFactory GetRepositoryFactory()
		{
			return repositoryFactory;
		}
	}
}
