using System;
using PetaPoco;
namespace Entity.Model
{
	
	[TableName("User")]
    [PrimaryKey("Id", AutoIncrement = false)]
    [ExplicitColumns]
	public class User
	{
		public User()
		{
			this.Id = Guid.NewGuid().ToString(); 
		}
			
		/// <summary>
		/// Id
        /// </summary>		
		[Column]
        public string Id { get; set; }
				
		/// <summary>
		/// UserName
        /// </summary>		
		[Column]
        public string Name { get; set; }

        /// <summary>
        /// Account
        /// </summary>		
        [Column]
        public string Account { get; set; }
				
		/// <summary>
		/// Password
        /// </summary>		
		[Column]
        public string Password { get; set; }

        /// <summary>
        /// Sex
        /// </summary>		
        [Column]
        public int Sex { get; set; }

        [Column]
        public int Status { get; set; }
		   
	}
}