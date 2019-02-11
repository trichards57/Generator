using Generator.Model;
using System.IO;
using System.Linq;

namespace Generator.Generators
{
    internal class FormTestsGenerator : Generator
    {
        public static void WriteFormTests(string path, Form form)
        {
            using (var file = new StreamWriter(path))
            {
                WriteHeaders(file, form);
                WriteTestHandleInput(file, form);
                WriteTestDescription(file, form);
                WriteNoDataTest(file, form);
                WriteHandleInputTests(file, form);
                if (form.UsesUpdateMode)
                {
                    WriteValidationTest(file, form, true, true);
                    WriteValidationTest(file, form, false, true);
                    WriteValidationTest(file, form, true, false);
                    WriteValidationTest(file, form, false, false);
                }
                else
                {
                    WriteValidationTest(file, form, true);
                    WriteValidationTest(file, form, false);
                }
                WriteEndTests(file, form);
            }
        }

        private static void WriteEndTests(TextWriter writer, Form form)
        {
            writer.WriteLine("});");
        }

        private static void WriteHandleInputTests(TextWriter writer, Form form)
        {
            foreach (var field in form.Fields.OrderBy(f => f.Name))
            {
                writer.WriteLine($"  it(\"should call store's handleInput on {field.Name} change\", () => {{");
                writer.WriteLine($"    testHandleInput(\"{field.Name}\");");
                writer.WriteLine("  });");
                writer.WriteLine();
            }
        }

        private static void WriteHeaders(TextWriter writer, Form form)
        {
            var testSupportPath = $"{PathToSourceRoot(form.PathLevel)}__test-support__";
            var storeInterfacesPath = $"{PathToSourceRoot(form.PathLevel)}store/interfaces";

            writer.WriteLine("jest.mock(\"@material-ui/core/Modal\");");
            writer.WriteLine();
            writer.WriteLine("import Modal from \"@material-ui/core/Modal\";");
            writer.WriteLine("import { shallow } from \"enzyme\";");
            writer.WriteLine("import React from \"react\";");
            writer.WriteLine("import ReactDOM from \"react-dom\";");
            writer.WriteLine("import TestUtils from \"react-dom/test-utils\";");
            writer.WriteLine("import renderer from \"react-test-renderer\";");

            if (!string.IsNullOrWhiteSpace(form.StoreType))
                writer.WriteLine($"import {{ createMock{form.StoreType.Substring(1)} }} from \"{testSupportPath}/change-stores\";");
            else if (form.CustomStoreGenerator)
                writer.WriteLine($"import {{ createMock{form.Name}Store }} from \"{testSupportPath}/change-stores\";");
            else
                writer.WriteLine($"import {{ createMockChangeStore }} from \"{testSupportPath}/change-stores\";");

            writer.WriteLine($"import {{ mockComponent }} from \"{testSupportPath}/mock-component\";");
            writer.WriteLine($"import {{ createTestEvent }} from \"{testSupportPath}/test-event\";");

            if (!form.CustomStoreGenerator)
                writer.WriteLine($"import {{ {form.StoreData} }} from \"{storeInterfacesPath}/{form.KebabName}\";");

            writer.WriteLine($"import {{ {form.FormName} }} from \"./index\";");
            writer.WriteLine();
            writer.WriteLine("mockComponent(Modal as TestUtils.MockedComponentClass);");
            writer.WriteLine();
        }

        private static void WriteNoDataTest(TextWriter writer, Form form)
        {
            writer.WriteLine("  it(\"should render no data without error\", () => {");
            writer.WriteLine("    const div = document.createElement(\"div\");");
            writer.WriteLine();
            writer.WriteLine("    ReactDOM.render(");
            writer.WriteLine($"      <{form.FormName}");

            if (!string.IsNullOrWhiteSpace(form.StoreType))
                writer.WriteLine($"        {form.Store}={{createMock{form.StoreType.Substring(1)}()}}");
            else if (form.CustomStoreGenerator)
                writer.WriteLine($"        {form.Store}={{createMock{form.Name}Store()}}");
            else
                writer.WriteLine($"        {form.Store}={{createMockChangeStore()}}");

            writer.WriteLine($"      />,");
            writer.WriteLine($"      div");
            writer.WriteLine("    );");
            writer.WriteLine();
            writer.WriteLine("    ReactDOM.unmountComponentAtNode(div);");
            writer.WriteLine("  });");
            writer.WriteLine();
        }

        private static void WriteTestDescription(TextWriter writer, Form form)
        {
            writer.WriteLine($"describe(\"{form.HumanName}\", () => {{");
        }

        private static void WriteTestHandleInput(TextWriter writer, Form form)
        {
            writer.WriteLine("function testHandleInput(name: string) {");

            if (!string.IsNullOrWhiteSpace(form.StoreType))
                writer.WriteLine($"  const mockStore = createMock{form.StoreType.Substring(1)}(");
            else if (form.CustomStoreGenerator)
                writer.WriteLine($"  const mockStore = createMock{form.Name}Store(");
            else
                writer.WriteLine($"  const mockStore = createMockChangeStore<{form.StoreData}>(");

            writer.WriteLine($"    createTestEvent()");
            writer.WriteLine("  );");
            writer.WriteLine();
            writer.WriteLine("  const control = shallow(");
            writer.WriteLine($"    <{form.FormName} {form.Store}={{mockStore}} />");

            if (form.NeedsStyles)
                writer.WriteLine("  ).dive().dive();");
            else
                writer.WriteLine("  ).dive();");

            writer.WriteLine();
            writer.WriteLine("  const testParam = \"Test Param\";");
            writer.WriteLine();
            writer.WriteLine("  control.find(`#${name}`).simulate(\"change\", testParam);");
            writer.WriteLine();
            writer.WriteLine("  expect(mockStore.handleInput).toBeCalledWith(testParam);");
            writer.WriteLine("}");
            writer.WriteLine();
        }

        private static void WriteValidationTest(TextWriter writer, Form form, bool valid, bool? updateMode = null)
        {
            var validText = valid ? "valid" : "invalid";

            if (updateMode == true)
                writer.WriteLine($"  it(\"should render {validText} update inputs correctly\", () => {{");
            else if (updateMode == false)
                writer.WriteLine($"  it(\"should render {validText} create inputs correctly\", () => {{");
            else
                writer.WriteLine($"  it(\"should render {validText} inputs correctly\", () => {{");

            if (!string.IsNullOrWhiteSpace(form.StoreType))
                writer.WriteLine($"    const mockStore = createMock{form.StoreType.Substring(1)}(");
            else if (form.CustomStoreGenerator)
                writer.WriteLine($"    const mockStore = createMock{form.Name}Store(");
            else
                writer.WriteLine($"    const mockStore = createMockChangeStore<{form.StoreData}>(");

            writer.WriteLine($"      createTestEvent()");
            writer.WriteLine("    );");
            writer.WriteLine("    mockStore.show = true;");

            if (updateMode != null)
                writer.WriteLine($"    mockStore.updateMode = {updateMode.ToString().ToLower()};");

            writer.WriteLine("    mockStore.validation = {");
            writer.WriteLine($"      allValid: {valid.ToString().ToLowerInvariant()},");

            foreach (var field in form.Fields.OrderBy(f => f.Name))
            {
                writer.WriteLine($"      {field.Name}: {valid.ToString().ToLowerInvariant()},");
            }

            writer.WriteLine("    };");
            writer.WriteLine();
            writer.WriteLine("    const tree = renderer.create(");
            writer.WriteLine($"      <{form.FormName} {form.Store}={{mockStore}} />");
            writer.WriteLine("    );");
            writer.WriteLine();
            writer.WriteLine("    expect(tree).toMatchSnapshot();");
            writer.WriteLine("  });");
            writer.WriteLine();
        }
    }
}