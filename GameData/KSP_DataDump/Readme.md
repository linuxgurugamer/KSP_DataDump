KSP_DataDump

This mod is intended for Tech Tree authors and others who are looking to balance various parts.  It is intended to allow the user to dump all the desired values from parts and part modules to a CSV file, which you can then import into Microsoft Excel, Google Sheets or any other spreadsheet or database system.

The mod is only available in the Editor. 


Mod Selection

Click the toolbar button (button image here) to open up the Mod Selection window.
The window opens and displays a list of all the installed mods.  Note that it goes by the directory name, so some mods' names aren't obvious sometimes

I recommend you only work on one mod at a time, and the discussion following will assume that.  There is nothing to prevent you from dumping the data for multiple mods, it can be confusing if there is too much information.

Select a mod by clicking on the button.  The little toggle to the left of the buttons are only an indication that they have been selected or not.  This UI concept is used in the next two windows as well.


Module selection

When you click the button, a new window will open up, the Module Selection window.  While it will be opened at the right edge of the Mod Selection window, it can be freely moved around for better placement if you like.  The UI is the same as for the ModSelection  This window will show all the part modules in the selected mod.  


Field Selection

Click on one of the part modules listed in the ModuleSelection window.  A new window will open up below, the Field Selection window.  While the UI is the same, the data presented is a little different.  In each button the field name will be displayed, followed by a colon and then followed by an example data.  The example data is pulled from one of the parts in the mod which uses that part module.  The window also has 4 buttons at the bottom:  OK, Select All, Select None, and Toggle All.  These are intended as shortcuts, and do exactly what they say.  The Toggle All will flip all of them, so those which are selected are deselected, and those which were not selected become selected.
Note that if you click other part modules in the second window, the fields from that part module will replace any of the fields that were previously were in the Field Selection.  All selections are remembered.

Click OK to close the Field Selection, and OK to close the Module selection windows

When you are ready, close the Field and Module selection, and then click the Export button.  A small window will open up where you can enter a file name.  Note that  the characters  ".csv" are already there, so just start typing the filename at the beginning.  (yes, if you click at the end of the name, harmless and funny things will happen).  A new feature is that the first selected mod will be used to present a suggested filename.
There is also a toggle which you can select to have each mod's data be outputted into individual unique files, consisting of the mod name with ".csv" appended.

The files are written to the main KSP game directory

When the export is complete, you will be notified and will need to acknowledge the export.