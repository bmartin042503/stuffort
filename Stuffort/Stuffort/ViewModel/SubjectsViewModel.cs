using Stuffort.Model;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Stuffort.ViewModel
{
    public class SubjectsViewModel 
    {
        public SubjectCommand SubjectCommand { get; set; }
        public ObservableCollection<Subject> SubjectList { get; set; }
        public SubjectsViewModel()
        {
            SubjectList = new ObservableCollection<Subject>();
            SubjectCommand = new SubjectCommand(this);
        }

        public async Task NavigateToNewSubject()
        {
            await AppShell.Current.GoToAsync($"{nameof(NewSubjectPage)}");
        }

        public async Task UpdateSubjects()
        {
            SubjectList.Clear();
            var subjectList = await SubjectServices.GetSubjects();
            foreach (var subject in subjectList)
                SubjectList.Add(subject);
        }
    }
}
