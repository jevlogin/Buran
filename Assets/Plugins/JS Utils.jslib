var plugin = {

    OpenTab : function(url)
    {
        url = UTF8ToString(url);
        window.open(url,'_blank');
    },
    
	GoBack: function () 
	{
        window.history.back();
    },

	ExitGame: function () 
	{
        window.parent.postMessage("exitGame", "*");
    },

    ShowButonStart: function()
    {
    	window.parent.postMessage("ShowButonStart", "*");
    }
};
mergeInto(LibraryManager.library, plugin);