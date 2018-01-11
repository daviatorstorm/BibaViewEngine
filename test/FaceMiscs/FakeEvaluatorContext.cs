namespace test.FaceMiscs
{
    public class FakeEvaluatorContext
    {
        public string WelcomeMessage { get; set; } = "Welcome to Testin";
        public LevelOne LevelOne { get; set; } = new LevelOne();
    }

    public class LevelOne
    {
        public string Message { get; set; } = "LevelOne";
        public LevelTwo LevelTwo { get; set; } = new LevelTwo();
    }

    public class LevelTwo
    {
        public string Message { get; set; } = "LevelTwo";
        public LevelThree LevelThree { get; set; } = new LevelThree();
    }

    public class LevelThree
    {
        public string Message { get; set; } = "LevelThree";
    }
}
