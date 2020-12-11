using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapstoneUWP.Model.Repo_Model;

namespace CapstoneUWP.Model.Display_Model
{
    public class GradeItemGradeDisplay
    {
        public GradeItem GradeItem { get; set; }

        public String Name { get; set; }

        public List<GradeItemGrade> UngradedItems { get; set; }

        public int UngradedCount { get; set; }

        public List<GradeItemGrade> GradedItems { get; set; }

        public int GradedCount { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="GradeItemGradeDisplay"/> class.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        /// <param name="ungradedItems">The ungraded items.</param>
        /// <param name="gradedItems">The graded items.</param>
        public GradeItemGradeDisplay(GradeItem gradeItem, List<GradeItemGrade> ungradedItems, List<GradeItemGrade> gradedItems)
        {
            this.GradeItem = gradeItem;
            this.Name = gradeItem.Name;
            this.UngradedItems = ungradedItems;
            this.GradedItems = gradedItems;
            this.UngradedCount = ungradedItems.Count;
            this.GradedCount = gradedItems.Count;
        }
    }
}
