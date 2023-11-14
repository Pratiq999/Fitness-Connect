using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitnessConnect.Controllers;
using FitnessConnect.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Protocol;

namespace FitnessConnectTest;

[TestClass]
public class UnitTest1
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICommonRepository _commonrepo;
    private readonly IHostingEnvironment _environment;

    [TestMethod]
    public async Task TestGenerateExercise()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = await controller.GenerateExercise("", "", "", "", CancellationToken.None);

        Assert.AreNotEqual(result.ToJToken()["ViewData"]?["ExerciseData"].ToArray().Length, 0);
    }

    [TestMethod]
    public async Task TestGenerateExercise1()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result =
            await controller.GenerateExercise("something that doesn't exists", "", "", "", CancellationToken.None);

        Assert.Equals(result.ToJToken()["ViewData"]?["ExerciseData"].ToArray().Length, 0);
    }

    [TestMethod]
    public async Task TestGenerateExercise2()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = await controller.GenerateExercise("", "", "", "beginner", CancellationToken.None);

        Assert.AreEqual(
            result.ToJToken()["ViewData"]?["ExerciseData"].ToArray()
                .Where(t => t.ToObject<ExerciseModel>().Difficulty != "beginner").ToList().Count, 0);
    }

    [TestMethod]
    public async Task TestGenerateExercise3()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = await controller.GenerateExercise("", "", "calves", "beginner", CancellationToken.None);

        Assert.AreEqual(
            result.ToJToken()["ViewData"]?["ExerciseData"].ToArray().Where(t =>
                    t.ToObject<ExerciseModel>().Difficulty != "beginner" &&
                    t.ToObject<ExerciseModel>().Muscle != "calves")
                .ToList().Count, 0);
    }

    [TestMethod]
    public async Task TestGenerateExercise4()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = await controller.GenerateExercise("", "strength", "calves", "beginner", CancellationToken.None);

        Assert.AreEqual(result.ToJToken()["ViewData"]?["ExerciseData"].ToArray().Where(t =>
        {
            var model = t.ToObject<ExerciseModel>();
            
            return model.Difficulty != "beginner" &&
                   model.Muscle != "calves" && model.Type != "strength";
        }).ToList().Count, 0);
    }
    
    [TestMethod]
    public void TestError()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = controller.Index();

        Assert.AreEqual(result.ToJToken().Count(), 1);
    }
    
    [TestMethod]
    public void TestPrivacy()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = controller.Privacy();

        Assert.AreEqual(result.ToJToken().Count(), 1);
    }
    
    [TestMethod]
    public void TestAboutUs()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = controller.AboutUs();

        Assert.AreEqual(result.ToJToken().Count(), 1);
    }
    
    [TestMethod]
    public void TestContactUs()
    {
        var controller = new HomeController(_logger, _commonrepo);
        var result = controller.ContactUs();

        Assert.AreEqual(result.ToJToken().Count(), 1);
    }
}