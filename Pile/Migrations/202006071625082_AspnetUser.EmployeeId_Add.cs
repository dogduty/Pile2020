namespace Pile.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspnetUserEmployeeId_Add : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "EmployeeId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "EmployeeId", c => c.Int(nullable: false));
        }
    }
}
