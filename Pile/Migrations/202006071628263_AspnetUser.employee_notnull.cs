namespace Pile.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspnetUseremployee_notnull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "EmployeeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "EmployeeId", c => c.Int());
        }
    }
}
