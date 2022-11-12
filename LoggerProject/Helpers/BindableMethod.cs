﻿using System;

namespace Helpers
  {

  /// <summary>
  /// Decorates method that is eligible for binding inside XAML.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class BindableMethod : Attribute
    {

    /// <summary>
    /// Gets or sets the bindable XAML name.
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"> Bindable XAML name. </param>
    public BindableMethod(string name)
      {
      Name = name;
      }
    }
  }
