using System;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Singulink.WPF.Data.Tests
{
    [TestClass]
    public class ExplicitTargetTests : TracedTests
    {
        [STATestMethod]
        public void NullTarget()
        {
            var binding = new MethodBindingExtension(new NullExtension(), "SomeMethod");
            TestHelper.RunMethodBinding(null, binding);

            TraceMessages.ShouldBe(new[] {
                "testhost Warning: 0 : [MethodBindingExtension] First method binding argument is required and cannot resolve to null - method name or method target expected.",
            });
        }

        [STATestMethod]
        public void NullMethodName()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, new NullExtension());
            TestHelper.RunMethodBinding(null, binding);

            binding = new MethodBindingExtension(vm, null);
            TestHelper.RunMethodBinding(null, binding);

            TraceMessages.ShouldBe(new[] {
                "testhost Warning: 0 : [MethodBindingExtension] Method target type resolved to 'Singulink.WPF.Data.Tests.ViewModel', method name resolved to null.",
                "testhost Warning: 0 : [MethodBindingExtension] Method target type resolved to 'Singulink.WPF.Data.Tests.ViewModel', method name resolved to null.",
            });
        }

        [STATestMethod]
        public void MissingMethod()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, "NonExistent");
            TestHelper.RunMethodBinding(null, binding);

            binding = new MethodBindingExtension(vm, "NonExistent", null);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(false);
            TraceMessages.ShouldBe(new[] {
                "testhost Warning: 0 : [MethodBindingExtension] Could not find parameterless method 'NonExistent' (target type 'Singulink.WPF.Data.Tests.ViewModel').",
                "testhost Warning: 0 : [MethodBindingExtension] Could not find method 'NonExistent' (target type 'Singulink.WPF.Data.Tests.ViewModel') that accepts the provided arguments (null).",
            });
        }

        [STATestMethod]
        public void NoParameters()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.NoParameters));
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void IntParameter()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.IntParam), 5);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void IntParameterAsXamlString()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.IntParam), "5");
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void IntParameterAsInvalidXamlString()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.IntParam), "5x");
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(false);
            TraceMessages.Count.ShouldBe(1);
            TraceMessages[0].ShouldStartWith("testhost Warning: 0 : [MethodBindingExtension] Method 'IntParam' (target type 'Singulink.WPF.Data.Tests.ViewModel') parameter 1 (name: 'x', type: 'System.Int32') could not be assigned from XAML string argument '5x':");
        }

        [STATestMethod]
        public void IntParameterAsNull()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.IntParam), null);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(false);
            TraceMessages.ShouldBe(new[] {
                "testhost Warning: 0 : [MethodBindingExtension] Method 'IntParam' (target type 'Singulink.WPF.Data.Tests.ViewModel') parameter 1 (name: 'x', type: 'System.Int32') is not assignable to null.",
            });
        }

        [STATestMethod]
        public void IntParameterAsNullExtension()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.IntParam), new NullExtension());
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(false);
            TraceMessages.ShouldBe(new[] {
                "testhost Warning: 0 : [MethodBindingExtension] Method 'IntParam' (target type 'Singulink.WPF.Data.Tests.ViewModel') parameter 1 (name: 'x', type: 'System.Int32') is not assignable to null.",
            });
        }

        [STATestMethod]
        public void NullableIntParameter()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.NullableIntParam), 5);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int?));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void NullableIntParameterAsString()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.NullableIntParam), "5");
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int?));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void NullableIntParameterAsNull()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.NullableIntParam), null);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int?));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void NullableIntParameterAsNullExtension()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.NullableIntParam), new NullExtension());
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int?));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedIntStringAsString()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedIntString), "test");
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(string));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedIntStringAsInt()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedIntString), 5);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedIntStringAsNull()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedIntString), null);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(string));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedNullableIntDoubleAsInt()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedNullableIntDouble), 5);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int?));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedNullableIntDoubleAsDouble()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedNullableIntDouble), 5.0);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(double));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedNullableIntDoubleAsNull()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedNullableIntDouble), null);
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(true);
            vm.ParamType.ShouldBe(typeof(int?));
            TraceMessages.ShouldBeEmpty();
        }

        [STATestMethod]
        public void OverloadedNullableIntDoubleAsString()
        {
            var vm = new ViewModel();

            var binding = new MethodBindingExtension(vm, nameof(ViewModel.OverloadedNullableIntDouble), "5");
            TestHelper.RunMethodBinding(null, binding);

            vm.Executed.ShouldBe(false);
            TraceMessages.ShouldBe(new[] {
                "testhost Warning: 0 : [MethodBindingExtension] Could not find method 'OverloadedNullableIntDouble' (target type 'Singulink.WPF.Data.Tests.ViewModel') that accepts the provided arguments ('System.String').",
            });
        }
    }
}
