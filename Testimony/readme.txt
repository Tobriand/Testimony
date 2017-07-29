Testimony v1.0

Provides a relatively lightweight, slightly hacky, approach to specifying at design-
time the tests that should be performed on an interface or class, and ensuring that 
they are, in fact implemented. This is in response to the lack of built in code-coverage
functionality in Visual Studio Community Edition.

Core Functionality:
- Allows identification of interface members that should be tested *if* there exists a 
	test-class that claims to test them.
- Allows identifcation of tests that are NOT currently performed that have been specified
	as potentially needing to be performed.
- Allows specification by e.g managers or BAs of test interfaces that TestClasses can implement, 
	that themselves provide coverage.

Model:
	Product Interface	-->	Product Class
	<---->	
	Test Requirements Interface	-->	Test Interface --> Test Class

Usage:
- Reference this project/assembly in your test class
- Create a Coverage Tester [TestClass] which should inherit from ProvidesCoverageDefaultTestsBase.
	Any test method created in this class will be sufficient to ensure that all IProvidesCoverage<>
	implementations do in fact provide a test method for their coverage.
- Define Requirements:
	- Within your Test project, create an interface that inherits from the interface you want to test
	- To specify class level tests (e.g. a test for every method), decorate it with [ClassTestRequirements()]
	- To specify individual tests on methods, re-declare the method name, and decorate the newly declared
		method with [TestRequirement]
- Define an expected implementation:
	- Within your Test project, create an interface that inherits from IProvidesCoverage<TTested>
	- This is where the tests that should be implemented go.
	- Each test gets a [Covers] attribute to specify which member of TTested is being tested, and
		what condition the test takes place under. Note that this allows members that are NOT
		easy to just shove a [TestMethod] on top of to get tests, such as events or properties
- Create a class to perform your tests!
	- Create a [TestClass] for the object you want to test, have it inherit from ProvidesCoverage<TTested>.
	- NB if you have a separate base test class, you can instead simply implement IProvidesCoverage<TTested>,
		and in the implementation of ValidateCoverageTest, call this.ValidateCoverage() (it's an extension
		method that does it for you).
	- Inherit from any interfaces defining your tests.
	- Implement the interface
	- Annotate with [TestMethod] where appropriate
	- If you're testing a crazy-big class (and not going down the partial route), apply [SkipTest] attributes 
		to specify that you want to skip certain [TestRequirement]s as required.
	- Done!

Benefits:
- Interface can be written before the implementation
- Interface author can provide strong guidance as to what a correct implementation is by also providing tests.
- Tests can be written prior to interfaces, and subsequently be specified to cover tests vs interfaces.
- If your tests pass, you're covering everything you've said you will. If you're holding someone else to account,
	if *their* tests pass, then they do everything you told them to.
- You get nice clean output from ValidateCoverage telling you what's missing when something isn't, and
	what is skipped, where tests aren't performed.


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
