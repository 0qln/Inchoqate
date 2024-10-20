using Inchoqate.GUI.View.Editors.Edits.Properties;
using Inchoqate.GUI.View.Edits;
using Inchoqate.GUI.ViewModel.Edits;

namespace Inchoqate.GUI.View.Editors.Edits;

// Specifying the option controls for the edits this way allows to introduce 
// custom ordering of the options for each different edit.
// If this is needed someday, this is the way.

public class EditImplGrayscaleView(EditImplGrayscaleViewModel viewModel) : EditBaseView(viewModel, [
    new IntensityPropertyView { DataContext = viewModel, DelegationTarget = viewModel.DelegationTarget },
    new WeightsPropertyView { DataContext = viewModel, DelegationTarget = viewModel.DelegationTarget }
]);