using Microsoft.EntityFrameworkCore;
using UniversityDbApp.Data;
using UniversityDbApp.Models;

namespace UniversityDbApp.Tests;

public class DatabaseModelTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost;Database=NexusModelTests;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void TeacherBelongsToDepartmentAndDepartmentDeleteIsRestricted()
    {
        using var db = CreateContext();
        var fk = db.Model.FindEntityType(typeof(Teacher))!.GetForeignKeys()
            .Single(x => x.Properties.Single().Name == nameof(Teacher.DepartmentId));

        Assert.Equal(typeof(Department), fk.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
        Assert.True(fk.IsRequired);
    }

    [Fact]
    public void ProductBelongsToCategoryAndCategoryDeleteIsRestricted()
    {
        using var db = CreateContext();
        var fk = db.Model.FindEntityType(typeof(Product))!.GetForeignKeys()
            .Single(x => x.Properties.Single().Name == nameof(Product.CategoryId));

        Assert.Equal(typeof(Category), fk.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
        Assert.True(fk.IsRequired);
    }

    [Fact]
    public void DepartmentCodeAndTeacherEmailAreUnique()
    {
        using var db = CreateContext();
        var departmentIndex = db.Model.FindEntityType(typeof(Department))!.GetIndexes()
            .Single(x => x.Properties.Single().Name == nameof(Department.Code));
        var teacherIndex = db.Model.FindEntityType(typeof(Teacher))!.GetIndexes()
            .Single(x => x.Properties.Single().Name == nameof(Teacher.Email));

        Assert.True(departmentIndex.IsUnique);
        Assert.True(teacherIndex.IsUnique);
    }

    [Fact]
    public void ProductPriceUsesTwelveDigitsAndTwoDecimalPlaces()
    {
        using var db = CreateContext();
        var price = db.Model.FindEntityType(typeof(Product))!.FindProperty(nameof(Product.Price))!;

        Assert.Equal(12, price.GetPrecision());
        Assert.Equal(2, price.GetScale());
    }
}
