# NDiff

A sequence comparison library for any object.

## Description

A C\# implementation of the Longest Common Subsequence algorithm for both text and objects.

The code is adapted from the original code at [Mathertel](http://www.mathertel.de/Diff/).

The original code provided an algorithm implementation for strings. NDiff extends this to handle all types. If the type being compared does not implement ```IEquatable<T>```, then an ```IEqualityComparer<T>``` should be provided.

## Runtime Environment

The project is built against under .NET Standard 2.0, 2.1 and .NET 6.0.

## Using the Library

Comparison operations are implemented as extension methods on either strings or enumerables (```IEnumerable<T>```). Calling the ```Diff``` extension method will produce a set of ```DiffEntry``` objects describing all differences between the compared sequences.

Using the ```Format``` extension method will provide a list of ```Compared<T>```. This list gives all items in the source list and other list and marks them with the ```ChangeAction``` for that item. This lets you generate a list of deletions and insertions for any item type.

Similar formatting options are provided for strings, using the ```DiffFormat``` extension methods.

For the moment, please refer to the tests for usage examples. There are tests for both text and object sequences.

## Reporting Issues and Bugs

When reporting issues and bugs, please provide a clear set of steps to reproduce the issue. The best way is to provide a failing test case as a pull request.

If that is not possible, please provide a set of steps which allow the bug to be reliably reproduced. These steps must also reproduce the issue on a computer that is not your own.

## Contributions

All contributions are appreciated. Please provide them as an issue with an accompanying pull request.

This is an open source project. Work has gone into the [project](http://www.mathertel.de/Diff/) it was forked from, as well as the later improvements. Please respect the license terms and the fact that issues and contributions may not be handled as fast as you may wish. The best way to get your contribution adopted is to make it easy to pull into the code base.
