window.removeResizeDiv = function(e){
  console.log(e)
  var parentDiv = e.offsetParent; parentDiv.parentNode.removeChild(parentDiv);
}

document.querySelector('#createbtn').addEventListener('click',function(e){
  console.log(e)
  var coloumnName = document.querySelector('#kolon')
  document.getElementById('canvas-wrap').innerHTML += '<div class="resize-drag"><span class="close" onclick="removeResizeDiv(this)">&times;</span></div>';
  changePageNumber(muratPageNumberNow)
})

var muratPdf;
var muratPdfNumbers;
var muratPageNumberNow;

window.goBack= function goBack(){
    if(muratPageNumberNow === 1){
      return;
    }
    muratPageNumberNow = muratPageNumberNow-1;
    changePageNumber(muratPageNumberNow)
  }

  window.goForward = function goForward(){
    if(muratPageNumberNow === muratPdfNumbers){
      return;
    }
    muratPageNumberNow = muratPageNumberNow+1;
    changePageNumber(muratPageNumberNow)
  }

  
  function changePageNumber(pageNumber){
    pdfjsLib.getDocument(muratPdf).then(function(pdf) {
      // you can now use *pdf* here
      console.log("the pdf has ",pdf.numPages, "page(s).")
      pdf.getPage(pageNumber).then(function(page) {
        // you can now use *page* here
        var viewport = page.getViewport(1.3338);
        viewport.height=1122.519685039;
        viewport.width=793.700787402;
        var canvas = document.querySelector("canvas")
        canvas.height = viewport.height;
        canvas.width = viewport.width;


        page.render({
          canvasContext: canvas.getContext('2d'),
          viewport: viewport
        });
      });

    });
  }

document.querySelector("#pdf-upload").addEventListener("change", function(e){
	var canvasElement = document.querySelector("canvas")
	var file = e.target.files[0]
	if(file.type != "application/pdf"){
		console.error(file.name, "is not a pdf file.")
		return
	}
	
 
	var fileReader = new FileReader();  

  

	fileReader.onload = function() {
		var typedarray = new Uint8Array(this.result);
    muratPdf = typedarray;
		pdfjsLib.getDocument(typedarray).then(function(pdf) {
			// you can now use *pdf* here
			console.log("the pdf has ",pdf.numPages, "page(s).")
      muratPageNumberNow = 1;
      muratPdfNumbers = pdf.numPages;

			pdf.getPage(1).then(function(page) {
				// you can now use *page* here
				var viewport = page.getViewport(1.3338);
				viewport.height=1122.519685039;
				viewport.width=793.700787402;
				var canvas = document.querySelector("canvas")
				canvas.height = viewport.height;
				canvas.width = viewport.width;


				page.render({
					canvasContext: canvas.getContext('2d'),
					viewport: viewport
				});
			});

		});
	};

	fileReader.readAsArrayBuffer(file);
})



 function dragMoveListener (event) {
  var div = document.getElementsByClassName('resize-drag')[0];

    var target = event.target,
        // keep the dragged position in the data-x/data-y attributes
        x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
        y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

    // translate the element
    target.style.webkitTransform =
    target.style.transform =
      'translate(' + x + 'px, ' + y + 'px)';

    // update the posiion attributes
    target.setAttribute('data-x', x);
    target.setAttribute('data-y', y);
  }

   window.dragMoveListener = dragMoveListener;


interact('.draggable')
  .draggable({
    ignoreFrom: '.ignore',
    // enable inertial throwing
    inertia: true,
    // keep the element within the area of it's parent
    restrict: {
      //restriction: "parent",
      endOnly: true,
      elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
    },
    // enable autoScroll
    autoScroll: true,

    // call this function on every dragmove event
    onmove: dragMoveListener,
  });
  

   interact('.resize-drag')
  .draggable({
    onmove: window.dragMoveListener,
    restrict: {
      restriction: 'parent',
      elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
    },
  })
  .resizable({
    // resize from all edges and corners
    edges: { left: true, right: true, bottom: true, top: true },

    // keep the edges inside the parent
    restrictEdges: {
      outer: 'parent',
      endOnly: true,
    },

    // minimum size
    restrictSize: {
      min: { width: 40, height: 20 },
    },

    inertia: true,
  })
  .on('resizemove', function (event) {
    
    var target = event.target,
        x = (parseFloat(target.getAttribute('data-x')) || 0),
        y = (parseFloat(target.getAttribute('data-y')) || 0);
	
    // update the element's style
    target.style.width  = event.rect.width + 'px';
    target.style.height = event.rect.height + 'px';


    // translate when resizing from top or left edges
    x += event.deltaRect.left;
    y += event.deltaRect.top;

    target.style.webkitTransform = target.style.transform =
        'translate(' + x + 'px,' + y + 'px)';
    
    target.setAttribute('data-x', x);
    target.setAttribute('data-y', y);
    //target.textContent =  Math.round(event.rect.width*72/96) + '\u00D7' + Math.round(event.rect.height*72/96);
  })
  .on('resizeend',getPointXY)
  .on('dragend',getPointXY);


function getPointXY(event){

  var target = event.currentTarget;
  var div = document.getElementsByClassName('resize-drag')[0];
  var parentPos = document.getElementById('canvas-wrap').getBoundingClientRect(),
      childrenPos = target.getBoundingClientRect(),
      relativePos = {};

  relativePos.centerX =  Math.round((childrenPos.left - parentPos.left+Math.round(div.offsetWidth/2))*72/96) //595,5
  relativePos.centerY =Math.round( Math.abs( Math.round((childrenPos.top - parentPos.top+Math.round(div.offsetHeight/2))*72/96)-841.5)) //841,5
  relativePos.y1 = Math.round(Math.abs( Math.round(childrenPos.top - parentPos.top)*72/96 -841.5)),
  relativePos.x1 = Math.abs( Math.round(parentPos.right - childrenPos.right)*72/96-595.5),
  relativePos.y0 = Math.round(Math.round(Math.round(parentPos.bottom-childrenPos.bottom)*72/96)),
  relativePos.x0 = Math.round(Math.round(childrenPos.left - parentPos.left)*72/96);


    target.setAttribute('data-y1', relativePos.y1);
    target.setAttribute('data-x1', relativePos.x1);
    target.setAttribute('data-y0', relativePos.y0);
    target.setAttribute('data-x0', relativePos.x0);
    target.setAttribute('data-xCenter', relativePos.centerX);
    target.setAttribute('data-yCenter', relativePos.centerY);

  return relativePos;
}