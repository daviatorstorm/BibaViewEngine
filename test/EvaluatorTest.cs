using BibaViewEngine.Compiler;
using BibaViewEngine.Exceptions;
using test.FaceMiscs;
using Xunit;

namespace test
{
    public class EvaluatorTest
    {
        Evaluator evaluator;
        string rootExpression = "WelcomeMessage";
        string levelOneExpression = "LevelOne.Message";
        string levelTwoExpression = "LevelOne.LevelTwo.Message";
        string levelThreeExpression = "LevelOne.LevelTwo.LevelThree.Message";
        FakeEvaluatorContext context = new FakeEvaluatorContext();

        public EvaluatorTest()
        {
            evaluator = Evaluator.Create();
        }

        [Fact]
        public void Evaluate_RootLevel_Success()
        {
            Assert.Equal(context.WelcomeMessage, evaluator.Evaluate(rootExpression, context));
        }

        [Fact]
        public void Evaluate_LevelOne_Success()
        {
            Assert.Equal(context.LevelOne.Message, evaluator.Evaluate(levelOneExpression, context));
        }

        [Fact]
        public void Evaluate_LevelTwo_Success()
        {
            Assert.Equal(context.LevelOne.LevelTwo.Message, evaluator.Evaluate(levelTwoExpression, context));
        }

        [Fact]
        public void Evaluate_LevelThree_Success()
        {
            Assert.Equal(context.LevelOne.LevelTwo.LevelThree.Message, evaluator.Evaluate(levelThreeExpression, context));
        }

        [Fact]
        public void Evaluate_Fail_PropertyNotExists()
        {
            Assert.Throws<EvaluatorPropertyNotExistsException>(() =>
            {
                evaluator.Evaluate("WellcomeMessage", context);
            });
        }

        ~EvaluatorTest()
        {
            evaluator.Dispose();
            context = null;
            rootExpression = string.Empty;
            levelOneExpression = string.Empty;
            levelTwoExpression = string.Empty;
            levelThreeExpression = string.Empty;
        }
    }
}
