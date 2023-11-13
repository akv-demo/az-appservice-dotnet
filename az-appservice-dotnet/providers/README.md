I prefer to layer platform depended code to "providers" because they
require integration tests;

The services are to be built from the providers and tested with unit tests
and mocked providers.

The sample of the service which uses the providers is
`StateProcessorService`.

The services expose "dependencies" as interfaces and the providers implement
them.