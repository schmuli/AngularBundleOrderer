# AngularBundleOrderer

A BundleOrderer implementation for ordering Angular application files

## File Names

The orderer expects files to be named using the dot notation

- app.controller.js
- logger.service.js
- skip.filter.js
- clock.directive.js

Additionally, each folder should contain a <code>module.js</code> file, which can optionally contain
the module name too: <code>app.module.js</code>.

Any files that don't match one of the known types: service, controller, directive, filter or module, will
not be sorted and will remain at the original position.

Note that the orderer does not diffrentiate between services and factories, these should both be marked 
as <code>.service</code>.

## Sort Order

The orderer currently assumes that module files should appear last, as the module file will include all it's components:

   ```
   angular.module('app', [])
       .service('logger', [LoggerService])
       .controller('AppController', [AppController])
       .filter('skip', skipFilter)
       .directive('clock', clockDirective);
  ``` 

