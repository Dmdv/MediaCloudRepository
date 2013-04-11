using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DataAccess.Test
{
	public class ServiceInterfaceProxy<TInterface> : ClientBase<TInterface> where TInterface : class
	{
		public ServiceInterfaceProxy(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		public ServiceInterfaceProxy(string endpointConfigurationName, IEnumerable<IEndpointBehavior> endpointBehaviors)
			: base(endpointConfigurationName)
		{
			if (endpointBehaviors != null)
				AddEndpointBehaviors(endpointBehaviors);
		}

		public ServiceInterfaceProxy(string endpointConfigurationName, string remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
		}

		public ServiceInterfaceProxy(Binding binding, string remoteAddress)
			: base(binding, new EndpointAddress(remoteAddress))
		{
		}

		public ServiceInterfaceProxy(Binding binding, string remoteAddress, IEnumerable<IEndpointBehavior> endpointBehaviors)
			: base(binding, new EndpointAddress(remoteAddress))
		{
			if (endpointBehaviors != null)
				AddEndpointBehaviors(endpointBehaviors);
		}

		public ServiceInterfaceProxy(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
		}

		public ServiceInterfaceProxy(Binding binding, EndpointAddress remoteAddress,
		                             IEnumerable<IEndpointBehavior> endpointBehaviors)
			: base(binding, remoteAddress)
		{
			if (endpointBehaviors != null)
				AddEndpointBehaviors(endpointBehaviors);
		}

		public void Dispose()
		{
			if (State == CommunicationState.Opened)
			{
				Close();
			}
			else
			{
				Abort();
			}
		}

		public TInterface GetInterface()
		{
			return Channel;
		}

		private void AddEndpointBehaviors(IEnumerable<IEndpointBehavior> endpointBehaviors)
		{
			foreach (var endpointBehavior in endpointBehaviors)
			{
				Endpoint.Behaviors.Add(endpointBehavior);
			}
		}
	}
}