Testimony v0.1

Provides a relatively lightweight, slightly hacky, approach to specifying at design-
time the tests that should be performed on an interface or class, and ensuring that 
they are, in fact implemented. This is in response to the lack of built in code-coverage
functionality in Visual Studio Community Edition.

Core Functionality:
- Allows identification of interface members that should be tested *if* there exists a 
	test-class that claims to test them.
- Allows identifcation of tests that are NOT currently performed that have been specified
	as potentially needing to be performed.

Usage:
- Reference this project/assembly in your test class
- Create a Coverage Tester [TestClass] which should inherit from ProvidesCoverageDefaultTestsBase.
	Any test method created in this class will be sufficient to ensure that all IProvidesCoverage<>
	implementations do in fact provide a test method for their coverage.
- Create a [TestClass] for the object you want to test, have it implement IProvidesCoverage<TTested>.
- Within TTested, or any interface it impliements, decorate each method you want to test 
	with a [TestRequirement], optionally specifying a name for the specific test that is required.
	See the Samples folder for an example of intended usage.

LICENSE DETAILS
Copyright (c) 2017 Toby Jacobs

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
