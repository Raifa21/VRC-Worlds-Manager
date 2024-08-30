using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class CreateGroupInstancePopup : ContentDialog
    {
        public CreateGroupInstancePopup(WorldModel world, string region)
        {
            this.InitializeComponent();
            this.DataContext = new CreateGroupInstancePopupViewModel(world, region);

            if(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.SelectGroupTitle.Text = "グループを選択";
                this.SelectGroupInstanceType.Text = "グループインスタンスタイプを選択";
                this.GroupTextSub_1.Text = "選択されたロールを持つユーザーと合流できます";
                this.GroupTextSub_2.Text = "選択されたロールを持つユーザーと合流できます";
                this.GroupPlusTextSub.Text = "インスタンス内のユーザーのフレンドは誰でも入ることができます";
                this.GroupPublicTextSub.Text = "誰でも入ることができます";
                this.SelectRoles.Text = "ロールを選択";
                this.Everyone.Text = "すべて（誰でもアクセス可能）";
                this.GroupInstanceOverview.Text = "グループインスタンスを作る";
                this.GroupConfirm.Text = "グループ";
                this.InstanceTypeConfirm.Text = "インスタンスタイプ";
                this.RolesConfirm.Text = "このインスタンスに合流できるロール：";
                this.InstanceQueueConfirm.Text = "満員の場合のインスタンス待機列";
                this.EnableInstanceQueue.Text = "インスタンス待機列";
                this.CreateInstanceButton.Content = "インスタンスを作る";
            }
            else
            {
                this.SelectGroupTitle.Text = "Select Group";
                this.SelectGroupInstanceType.Text = "Select Group Instance Type";
                this.GroupTextSub_1.Text = "Only the selected group roles may join";
                this.GroupTextSub_2.Text = "Only the selected group roles may join";
                this.GroupPlusTextSub.Text = "Any Friend of a user in the instance may join";
                this.GroupPublicTextSub.Text = "Anyone can join";
                this.SelectRoles.Text = "Select Roles";
                this.Everyone.Text = "Everyone";
                this.GroupInstanceOverview.Text = "Create Group Instance";
                this.GroupConfirm.Text = "Group";
                this.InstanceTypeConfirm.Text = "Instance Type";
                this.RolesConfirm.Text = "Roles that can join this instance:";
                this.InstanceQueueConfirm.Text = "Instance Queue when full";
                this.EnableInstanceQueue.Text = "Enable Instance Queue";
                this.CreateInstanceButton.Content = "Create Instance";
            }
        }
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void GroupSelected(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (sender != null)
            {
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                viewModel.GroupSelected(button.CommandParameter as string);
                if (button.Name == "Restricted")
                {
                    viewModel.IsRoleRestricted = true;
                }
            }
        }

        private void SelectType(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (sender != null)
            {
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                viewModel.AccessTypeSelected(button.Name);
                viewModel.IsGroupRolesLoadingComplete = false;
            }
        }

        private void ShowInstanceSelect(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.IsGroupRolesLoadingComplete = true;
        }

        private void HideInstanceSelect(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.IsGroupRolesLoadingComplete = false;
        }

        private void SelectRolesChanged_Checked(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.SelectRolesChanged();
        }
        private void SelectRolesChanged_Unchecked(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.SelectRolesChanged();
        }

        private void RolesSelected(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.RolesSelected();
        }

        private void InvertInstanceQueue(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.InvertInstanceQueue();
        }

        private void Region_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedRegion = radioButton.Content.ToString();
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                if(viewModel != null)
                {
                    viewModel.Region = selectedRegion;
                }
            }
        }
        private void CreateInstance(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.CreateInstanceAsync();

            this.Hide();
        }
    }
}
