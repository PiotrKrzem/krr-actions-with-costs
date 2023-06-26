using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Enum indicating type of the model item (FLUENT/ACTION).
    /// </summary>
    enum ModelElementType
    {
        FLUENT, ACTION
    }

    /// <summary>
    /// Class contains functionalities used in the Fluents/Actions section.
    /// </summary>
    class FluentActionView
    {
        // Input fields of fluents/actions
        private TextBox fluentTextBox;
        private TextBox actionTextBox;

        // Buttons of fluents/actions
        private Button fluentAddButton;
        private Button actionAddButton;
        private Button fluentRemoveButton;
        private Button actionRemoveButton;
        private Button fluentRemoveAllButton;
        private Button actionRemoveAllButton;

        // Checkboxed of fluents/actions
        private CheckedListBox fluentCheckBox;
        private CheckedListBox actionCheckBox;

        public FluentActionView(
            ref TextBox fluentTextBox, 
            ref TextBox actionTextBox, 
            ref Button fluentAddButton,
            ref Button actionAddButton,
            ref Button fluentRemoveButton,
            ref Button actionRemoveButton,
            ref Button fluentRemoveAllButton,
            ref Button actionRemoveAllButton,
            ref CheckedListBox fluentCheckBox,
            ref CheckedListBox actionCheckBox
            )
        {
            this.fluentTextBox = fluentTextBox;
            this.actionTextBox = actionTextBox;
            this.fluentAddButton = fluentAddButton;
            this.actionAddButton = actionAddButton;
            this.fluentRemoveButton = fluentRemoveButton;
            this.actionRemoveButton = actionRemoveButton;
            this.fluentRemoveAllButton = fluentRemoveAllButton;
            this.actionRemoveAllButton = actionRemoveAllButton;
            this.fluentCheckBox = fluentCheckBox;
            this.actionCheckBox = actionCheckBox;
        }


        /// <summary>
        /// Method updates the state of the AddButton for fluents/actions based on the content of input field.
        /// </summary>
        /// <param name="type">model item type (FLUENT/ACTION)</param>
        public void updateAddButtonState(ModelElementType type)
        {
            ref Button button = ref type == ModelElementType.ACTION ? ref actionAddButton : ref fluentAddButton;
            string text = (type == ModelElementType.ACTION ? actionTextBox : fluentTextBox).Text;
            button.Enabled = text.Length > 0;
        }

        /// <summary>
        /// Method adds new fluent/action to the collection after pressing "Enter".
        /// </summary>
        /// <param name="e">arguments of the event that triggered the method</param>
        /// <param name="type">model item type (FLUENT/ACTION)</param>
        /// <param name="elements">list of all fluents/actions</param>
        /// <param name="postAddCall">method executed after adding the fluent/action</param>
        /// <returns>boolean indicating if the element was successfuly added</returns>
        public bool addModelItemAfterEnter(ref KeyPressEventArgs e, ModelElementType type, List<string> elements, Action postAddCall)
        {
            string text = (type == ModelElementType.ACTION ? actionTextBox : fluentTextBox).Text;

            if (e.KeyChar == (char)Keys.Enter && text.Length != 0)
            {
                e.Handled = true;                   // supress sound after "Enter" 
                return type == ModelElementType.FLUENT ? addFluent(postAddCall, elements) : addAction(postAddCall, elements);
            }
            return false;
        }


        /// <summary>
        /// Method adds new fluent to the collection.
        /// </summary>
        /// <param name="postAddCall">method executed after adding the fluent</param>
        /// <param name="fluents">list of all fluents</param>
        /// <returns>boolean indicating if the fluent was successfuly added</returns>
        public bool addFluent(Action postAddCall, List<string> fluents)
        {
            if (fluentTextBox.Text.Contains("~"))
            {
                string message = "Fluent name should not contain \"~\" character. It has been reserved to be placed at the beginning to indicate fluent negation.";
                MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!fluents.Contains(fluentTextBox.Text))
            {
                fluents.Add(fluentTextBox.Text);
                fluentCheckBox.Items.Add(fluentTextBox.Text);
                fluentTextBox.Text = string.Empty;
            }
            else
            {
                string message = "This fluent was already added";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!fluentRemoveAllButton.Enabled)
            {
                fluentRemoveAllButton.Enabled = true;
            }

            postAddCall.Invoke();
            return true;
        }

        // <summary>
        /// Method adds new action to the collection.
        /// </summary>
        /// <param name="postAddCall">method executed after adding the fluent</param>
        /// <param name="actions">list of all actions</param>
        /// <returns>boolean indicating if the action was successfuly added</returns>
        public bool addAction(Action postAddCall, List<string> actions)
        {
            if (!actions.Contains(actionTextBox.Text))
            {
                actions.Add(actionTextBox.Text);
                actionCheckBox.Items.Add(actionTextBox.Text);
                actionTextBox.Text = string.Empty;
            }
            else
            {
                string message = "This action was already added";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!actionRemoveAllButton.Enabled)
            {
                actionRemoveAllButton.Enabled = true;
            }
            postAddCall.Invoke();
            return true;
        }

        /// <summary>
        /// Method updates the state of RemoveButton of fluents/actions.
        /// </summary>
        /// <param name="type">model item type (FLUENT/ACTION)</param>
        /// <param name="e">arguments of the event that triggered the method</param>
        public void updateRemoveButtonState(ModelElementType type, ItemCheckEventArgs e)
        {
            ref Button button = ref type == ModelElementType.ACTION ? ref actionRemoveButton : ref fluentRemoveButton;
            ref CheckedListBox checkBox = ref type == ModelElementType.FLUENT ? ref fluentCheckBox : ref actionCheckBox;

            button.Enabled = e.NewValue == CheckState.Checked || (checkBox.CheckedItems.Count > 1);
        }

        /// <summary>
        /// Method removes the fluent/action from the collection.
        /// </summary>
        /// <param name="type">model item type (FLUENT/ACTION)</param>
        /// <param name="elements">list of all fluents/actions</param>
        /// <param name="postDeleteCall">method executed after deleting the fluent</param>
        /// <returns>boolean indicating if the element was successfuly removed</returns>
        public bool deleteModelElement(ModelElementType type, List<string> elements, Action postDeleteCall)
        {
            ref CheckedListBox checkBox = ref type == ModelElementType.FLUENT ? ref fluentCheckBox : ref actionCheckBox;
            ref Button removeButton = ref type == ModelElementType.FLUENT ? ref fluentRemoveButton : ref actionRemoveButton;
            ref Button removeAllButton = ref type == ModelElementType.FLUENT ? ref fluentRemoveAllButton : ref actionRemoveAllButton;

            List<string> itemsToRemove = checkBox.CheckedItems.Cast<string>().ToList();

            foreach (string item in itemsToRemove)
            {
                checkBox.Items.Remove(item);
            }
            elements = checkBox.Items.Cast<string>().ToList();
            removeButton.Enabled = false;

            if (removeAllButton.Enabled && elements.Count == 0)
            {
                removeAllButton.Enabled = false;
            }

            postDeleteCall.Invoke();
            return true;
        }

        /// <summary>
        /// Method removes all fluents/actions.
        /// </summary>
        /// <param name="type">model item type (FLUENT/ACTION)</param>
        /// <param name="elements">list of all fluents/actions</param>
        /// <param name="postDeleteCall">method executed after deleting the fluent</param>
        /// <returns>boolean indicating if the elements were successfuly removed</returns>
        public bool deleteAllModelElementsOfType(ModelElementType type, List<string> elements, Action postDeleteCall)
        {
            ref CheckedListBox checkBox = ref type == ModelElementType.FLUENT ? ref fluentCheckBox : ref actionCheckBox;
            ref Button removeButton = ref type == ModelElementType.FLUENT ? ref fluentRemoveButton : ref actionRemoveButton;
            ref Button removeAllButton = ref type == ModelElementType.FLUENT ? ref fluentRemoveAllButton : ref actionRemoveAllButton;

            checkBox.Items.Clear();
            elements.Clear();
            removeAllButton.Enabled = false;
            removeButton.Enabled = false;

            postDeleteCall.Invoke();
            return true;
        }
    }
}
