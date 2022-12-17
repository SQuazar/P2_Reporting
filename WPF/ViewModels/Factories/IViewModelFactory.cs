using System;

namespace WPF.ViewModels.Factories;

public interface IViewModelFactory
{
    /// <summary>
    /// Creates view model from type
    /// </summary>
    /// <param name="modelType">Type of model</param>
    /// <exception cref="ArgumentException">Throws if factory cannot create instance from selected model type</exception>
    /// <returns></returns>
    ViewModelBase Create(object modelType);

    Type GetRequiredType();
}