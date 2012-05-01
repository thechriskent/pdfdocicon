'   Main.EventReceiver.vb
'   WireBear PDFdocIcon
'
'   Created by Chris Kent
'   Copyright 2012 WireBear. All rights reserved
'
#Region "License Information"
'   WireBear PDFdocIcon is free software: you can redistribute it and/or modify
'   it under the terms of the GNU General Public License as published by
'   the Free Software Foundation, either version 3 of the License, or
'   (at your option) any later version.
'
'   WireBear PDFdocIcon is distributed in the hope that it will be useful,
'   but WITHOUT ANY WARRANTY; without even the implied warranty of
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'   GNU General Public License for more details.
'
'   You should have received a copy of the GNU General Public License
'   along with WireBear PDFdocIcon - License.txt
'   If not, see <http://www.gnu.org/licenses/>.
#End Region

Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports System.IO
Imports Microsoft.SharePoint.Utilities
Imports System.Xml
Imports Microsoft.SharePoint.Administration

''' <summary>
''' This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("77290e9d-e6b9-4710-a6d1-aef6dfae6666")> _
Public Class MainEventReceiver
    Inherits SPFeatureReceiver

    Private _fileExtension As String = "pdf"
    Private _iconFileName As String = "ICPDF.png"

    Public Sub RunDocIconJob(Installing As Boolean, properties As SPFeatureReceiverProperties)
        Dim JobName As String = String.Format("DocIconJob_{0}", _fileExtension)

        'Ensure job doesn't already exist (delete if it does)
        Dim query = From job As SPJobDefinition In properties.Definition.Farm.TimerService.JobDefinitions Where job.Name.Equals(JobName) Select job
        Dim myJobDefinition As SPJobDefinition = query.FirstOrDefault()
        If myJobDefinition IsNot Nothing Then myJobDefinition.Delete()

        Dim myJob As New DocIconJob(JobName, SPFarm.Local.TimerService, Installing, _fileExtension, _iconFileName)

        'Get that job going!
        myJob.Title = String.Format("{0} icon mapping for {1}", IIf(Installing, "Adding", "Removing"), _fileExtension)
        myJob.Update()
        myJob.RunNow()
    End Sub


    Public Overrides Sub FeatureActivated(properties As Microsoft.SharePoint.SPFeatureReceiverProperties)
        RunDocIconJob(True, properties)
    End Sub

    Public Overrides Sub FeatureDeActivating(properties As Microsoft.SharePoint.SPFeatureReceiverProperties)
        RunDocIconJob(False, properties)
    End Sub

End Class
