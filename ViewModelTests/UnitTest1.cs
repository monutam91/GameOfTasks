using System;
using GameOfTasks.ViewModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace ViewModelTests
{
    [TestClass]
    public class UnitTest1
    {
        private AddJobViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _viewModel = new AddJobViewModel();
        }

        [TestMethod]
        public void NotifyTimeTest()
        {
            var date = _viewModel.NotifyDate;
            var time = _viewModel.NotifyTime;
            var settedTime = time.Add(new TimeSpan(1, 0, 0));
            _viewModel.NotifyTime = settedTime;
            Assert.IsTrue(_viewModel.NotifyDate.Equals(date) && !_viewModel.NotifyTime.Equals(time) &&
                          _viewModel.NotifyTime.Equals(settedTime));
        }
    }
}
