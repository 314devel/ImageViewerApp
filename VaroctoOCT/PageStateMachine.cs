using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaroctoOCT
{
    /// <summary>
    /// This class is responsible for managing the navigation of GUI pages. 
    /// </summary>
    class PageStateMachine
    {
        public enum PageType
        {
            None,
            Login,
            SelectPatient,
            ViewImages
        }

        /// <summary>
        /// This is a singleton instance of the PageStateMachine.
        /// </summary>
        public static PageStateMachine Instance = new PageStateMachine();

        private Stack<UserControl> _Pages = new Stack<UserControl>();

        private ContentPresenter _ContentPresenter = new ContentPresenter();

        private PageStateMachine()
        {
            ResetAndGoHome();
        }

        /// <summary>
        /// Gets and sets this object's ContentPresenter. If setting,
        /// the ContentPresenter.Content is set to the current page. 
        /// </summary>
        public ContentPresenter ContentPresenter
        {
            get { return _ContentPresenter; }
      
            set
            {
                _ContentPresenter = value;

                if (_ContentPresenter != null)
                {
                    // Set the Content Presenter to the current page.
                    _ContentPresenter.Content = _Pages.Peek();
                }
            }
        }

        public void ResetAndGoHome()
        {
            // Remove all pages from the stack
            _Pages.Clear();

            // Go back to the home page
            _Pages.Push(new LoginPage());

            // Set the page at the top of the stack to the current page
            ContentPresenter.Content = _Pages.Peek();
        }
        

        public void GotoPage(PageType newPage)
        {
            Type currentPageType = _Pages.Peek().GetType();

            if (Enum.IsDefined(typeof(PageType), newPage))
            {
                switch (newPage)
                {
                    case PageType.SelectPatient:
                        _Pages.Push(new SelectPatientPage());
                        break;
                        

                    case PageType.ViewImages:
                        _Pages.Push(new ViewImagesPage());
                        break;
                        

                    case PageType.None:
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }

                ContentPresenter.Content = _Pages.Peek();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns true if navigation backwards if allowed from the current page.
        /// </summary>
        public bool CanGoBackFromCurrentPage
        {
            get
            {
                // By default, the GUI can navigate back from the current page
                bool back = true;

                // Except in the case of the login and main menu pages.
                if (_Pages.Peek().GetType() == typeof(LoginPage))
                {
                    back = false;
                }

                return back;
            }
        }

        /// <summary>
        /// Goto the previous page. The contents of the current page will be discarded.
        /// </summary>
        public void GotoPrevPage()
        {
            // Pop the current page off the stack
            UserControl currPage = _Pages.Pop();
            
            // Set the page at the top of the stack to the current page
            System.Diagnostics.Debug.Assert(_Pages.Count > 0);
            
            if (_Pages.Count > 0)
            {
                UserControl lastPage = _Pages.Peek();

                // Some pages may require manually refreshing (such as updating 
                // data grids from the database)
                if (lastPage.GetType() == typeof(SelectPatientPage))
                {
                    (lastPage as SelectPatientPage).BindDataGrid();
                }

                ContentPresenter.Content = lastPage;


            }
        }
    }
}
