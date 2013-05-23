using System;
using DataAccess.Helpers;
using DataAccess.Extensions;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace DataAccess.Entities
{
	/// <summary>
	/// Пользователь.
	/// </summary>
	public sealed class User : TableServiceEntity
	{
		private Guid? _userId;
		private string _name;

		public User()
		{
		}

		/// <summary>
		/// Guid создается при создании, поэтому параметр необязательный.
		/// </summary>
		public User(string name, string password)
		{
			Guard.CheckContainsText(name, "name");
			Guard.CheckContainsText(password, "password");

			UserId = Guid.NewGuid();
			Name = name;
			Password = password;

			CreateKeys();
		}

		public Guid? UserId
		{
			get { return _userId; }
			set
			{
				_userId = value;
				RowKey = CreateRowKey();
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				PartitionKey = CreatePartitionKey();
			}
		}

		public string Password { get; set; }

		private string CreatePartitionKey()
		{
			return Name.Substring(0, 1);
		}

		private string CreateRowKey()
		{
			return UserId.HasValue && UserId != Guid.Empty
				       ? UserId.ToStringWithInvariantCulture()
				       : string.Empty;
		}

		private void CreateKeys()
		{
			PartitionKey = Name.Substring(0, 1);
			RowKey = CreateRowKey();
		}
	}
}