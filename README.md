# Introduction 

We work with different languages and different data representation protocols. 

SimplyWorks.Metadata is a set of abstractions and utilities that enable working with data of all binary forms from a unified business-level view.

A number in C# - for example - has a hefty amount of different representations with differing levels of capacity, accuracy and precision. All these types are needed to implement application features in an optimized way, but sometimes, we want to iron out all that and compare numbers regardless of their types. This is where the type 'Number' comes into play.

Another example is when we want to group a set of key value pairs that represent one business concept into a type. In C#, this can be achieved with one the following: 
- POCO with properties (Plain Old CLR Object)
- Dictionary
- Array of Key value pairs
- Tuples
- Dynamo objects
- ... etc.

We introduce the type 'DocumentKeyValueContainer' that allows us to deal the above in the same manner, a set of keys and values.

# Type System

Metadata introduces a unified type system of the following basic types:

- DocumentKeyValueContainer

- DocumentValueList

- Null

- Number

- Text

- Boolean

- DateTime

# Comparison / Validation

Also, Metadata provides a set of specifications / filter objects that can be used in combination to test whether a document satisfies certain conditions. Those include the following:

- AllFilter

- NoneFilter

- AndFilter

- OrFilter

- EqualToFilter

- ContainsFilter

- ... More to be implemented



The vision of Metadata is to provide a fabric that supports the creation of the following class of software:

- Document Search

- Analytics

- Schema Management Systems

- Content Management / Transformation Systems

- Content-based Message Routing

- Data Transformation Systems

- ... etc.


# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://www.visualstudio.com/en-us/docs/git/create-a-readme). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)