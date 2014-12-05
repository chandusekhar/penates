

namespace Penates.Models.ViewModels.Home {
    public class DashBoardViewModel {

        public DashBoardViewModel() {
            this.Error = false;
        }

        public string UserName { get; set; }

        public string Message { get; set; }

        public bool? Error { get; set; }

        public string Warning { get; set; }
    }
}