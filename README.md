# ZScanLib
A class library to make it easier to work with and manipulate process memory. 

### License
ZScanLib is a free software project licensed under the GPLv3 license. A copy of this license can be found in the project's root directory. 

### Requirements
* Windows Vista, 7, 8
* .NET Framework 4.5

### Contributing
Everyone is welcome to contribute to this project; just do your best to test the code you want to contribute, and I'll look over and merge your pull request. 

### Examples
The library treats searching virtual memory and module memory for a pattern the same way elevating you from dealing with 
virtual memory or access flags directly. 

```
// Get the first PlayOnline process. 
var process = GetProcessByID("pol").FirstOrDefault();

// Create a scanner that can read from the PlayOnline process. 
MemoryScanner scanner = new MemoryScanner(process);

// Create a signature to scan the process's memory for. 
Signature signature = new Signature(pattern, mask, offset)

// Scan for the address that matches the signature. 
IntPtr address = scanner.Scan(signature);
```
