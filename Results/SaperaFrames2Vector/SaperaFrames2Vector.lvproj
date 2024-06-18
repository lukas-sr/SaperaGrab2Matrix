<?xml version='1.0' encoding='UTF-8'?>
<Project Type="Project" LVVersion="20008000">
	<Property Name="varPersistentID:{217F1710-FD23-4614-83C2-386AFAD901CB}" Type="Ref">/My Computer/Network Variables Library.lvlib/ESL flag</Property>
	<Property Name="varPersistentID:{8EC1D00E-46F1-4005-B77B-ED097B282EE1}" Type="Ref">/My Computer/Network Variables Library.lvlib/ESL orders</Property>
	<Property Name="varPersistentID:{D7A9BEA2-E452-481B-9952-EF572D198877}" Type="Ref">/My Computer/Network Variables Library.lvlib/ESL data</Property>
	<Item Name="My Computer" Type="My Computer">
		<Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.tcp.enabled" Type="Bool">false</Property>
		<Property Name="server.tcp.port" Type="Int">0</Property>
		<Property Name="server.tcp.serviceName" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.tcp.serviceName.default" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
		<Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="specify.custom.address" Type="Bool">false</Property>
		<Item Name="Network Variables Library.lvlib" Type="Library" URL="../Network Variables Library.lvlib"/>
		<Item Name="Teste.vi" Type="VI" URL="../Teste.vi"/>
		<Item Name="Dependencies" Type="Dependencies">
			<Item Name="vi.lib" Type="Folder">
				<Item Name="FormatTime String.vi" Type="VI" URL="/&lt;vilib&gt;/express/express execution control/ElapsedTimeBlock.llb/FormatTime String.vi"/>
				<Item Name="subElapsedTime.vi" Type="VI" URL="/&lt;vilib&gt;/express/express execution control/ElapsedTimeBlock.llb/subElapsedTime.vi"/>
			</Item>
			<Item Name="DALSA.SaperaLT.SapClassBasic.dll" Type="Document" URL="../DALSA.SaperaLT.SapClassBasic.dll"/>
			<Item Name="GrabFrame4.dll" Type="Document" URL="../../../../Downloads/GrabTestTDNI/GrabFrame4.dll"/>
		</Item>
		<Item Name="Build Specifications" Type="Build">
			<Item Name="TesteTD" Type="EXE">
				<Property Name="App_copyErrors" Type="Bool">true</Property>
				<Property Name="App_INI_aliasGUID" Type="Str">{434F69BC-D452-41B7-9E8E-0E12D78A394A}</Property>
				<Property Name="App_INI_GUID" Type="Str">{4F60FAD8-37B1-4B7C-B83A-AF7F642990D3}</Property>
				<Property Name="App_serverConfig.httpPort" Type="Int">8002</Property>
				<Property Name="App_serverType" Type="Int">0</Property>
				<Property Name="Bld_autoIncrement" Type="Bool">true</Property>
				<Property Name="Bld_buildCacheID" Type="Str">{2DE36AE3-197A-4E89-9603-63B6E0C6B046}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">TesteTD</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">//WORKLAB-TD/Users/Admin/Documents/FST/TesteTD</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{D58C864A-F987-4E77-933B-523DDFD31223}</Property>
				<Property Name="Bld_version.build" Type="Int">47</Property>
				<Property Name="Bld_version.major" Type="Int">1</Property>
				<Property Name="Destination[0].destName" Type="Str">TesteTD.exe</Property>
				<Property Name="Destination[0].path" Type="Path">//WORKLAB-TD/Users/Admin/Documents/FST/TesteTD/TesteTD.exe</Property>
				<Property Name="Destination[0].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">Support Directory</Property>
				<Property Name="Destination[1].path" Type="Path">//WORKLAB-TD/Users/Admin/Documents/FST/TesteTD/data</Property>
				<Property Name="Destination[1].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="Source[0].itemID" Type="Str">{2EA56377-6EB3-4052-B13F-7AE6EF755AFD}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/My Computer/Teste.vi</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">VI</Property>
				<Property Name="SourceCount" Type="Int">2</Property>
				<Property Name="TgtF_companyName" Type="Str">Opto Eletrônica S/A</Property>
				<Property Name="TgtF_fileDescription" Type="Str">TesteTD</Property>
				<Property Name="TgtF_internalName" Type="Str">TesteTD</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">Copyright © 2024 Opto Eletrônica S/A</Property>
				<Property Name="TgtF_productName" Type="Str">TesteTD</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{3ED8DE4D-4117-41EF-B592-366F07BC556D}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">TesteTD.exe</Property>
				<Property Name="TgtF_versionIndependent" Type="Bool">true</Property>
			</Item>
		</Item>
	</Item>
</Project>
