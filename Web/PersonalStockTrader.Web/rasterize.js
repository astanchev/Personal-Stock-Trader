"use strict";
var page = require('webpage').create(),
    system = require('system'),
    address,
    output;

//console.log('Usage: rasterize.js [URL] [filename] [paperformat] [orientation]');
address = system.args[1];
output = system.args[2];

page.viewportSize = { width: 800, height: 600 };

page.paperSize = {
    format: system.args[3],
    orientation: system.args[4],
    margin: "2cm",
    header: {
        height: "2cm",
        contents: phantom.callback(function(pageNum, numPages) {
            //if (pageNum == 1) {
            //    return "";
            //}
            return "<h4>Personal Stock Trader <span style='float:right'>+359888888888</span></h4><hr/>";
        })
    },
    footer: {
        height: "1cm",
        contents: phantom.callback(function (pageNum, numPages) {
            return "<span style='float:right; font-family: sans-serif; font-size: 12px; font-weight: 400;'>" + pageNum + " / " + numPages + "</span>";
            
            //Other variant of footer
            //if (pageNum == numPages) {
            //    return "";
            //}
            //return "<h1>Footer <span style='float:right'>" + pageNum + " / " + numPages + "</span></h1>";
        })
    }
};

page.open(address, function (status) {
    if (status !== 'success') {
        console.log('Unable to load the address!');
        phantom.exit(1);
    } else {
        if (page.evaluate(function(){return typeof PhantomJSPrinting == "object";})) {
            paperSize = page.paperSize;
            paperSize.header.height = page.evaluate(function() {
                return PhantomJSPrinting.header.height;
            });
            paperSize.header.contents = phantom.callback(function(pageNum, numPages) {
                return page.evaluate(function(pageNum, numPages){return PhantomJSPrinting.header.contents(pageNum, numPages);}, pageNum, numPages);
            });
            paperSize.footer.height = page.evaluate(function() {
                return PhantomJSPrinting.footer.height;
            });
            paperSize.footer.contents = phantom.callback(function(pageNum, numPages) {
                return page.evaluate(function(pageNum, numPages){return PhantomJSPrinting.footer.contents(pageNum, numPages);}, pageNum, numPages);
            });
            page.paperSize = paperSize;
            console.log(page.paperSize.header.height);
            console.log(page.paperSize.footer.height);
        }
        window.setTimeout(function () {
            page.render(output);
            phantom.exit();
        }, 200);
    }
});