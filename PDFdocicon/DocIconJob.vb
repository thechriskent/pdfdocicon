'   DocIconJob.vb
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

Imports Microsoft.SharePoint.Administration
Imports System.IO
Imports Microsoft.SharePoint.Utilities
Imports System.Xml

Public Class DocIconJob
    Inherits SPServiceJobDefinition

#Region "Properties"

    Private _dociconPath As String
    Public ReadOnly Property DocIconPath() As String
        Get
            If String.IsNullOrEmpty(_dociconPath) Then _dociconPath = SPUtility.GetGenericSetupPath("TEMPLATE\XML\DOCICON.XML")
            Return _dociconPath
        End Get
    End Property

    Private Const InstallingKey As String = "DocIconJob_InstallingKey"
    Private Property _installing() As Boolean
        Get
            If Properties.ContainsKey(InstallingKey) Then
                Return Convert.ToBoolean(Properties(InstallingKey))
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            If Properties.ContainsKey(InstallingKey) Then
                Properties(InstallingKey) = value.ToString
            Else
                Properties.Add(InstallingKey, value.ToString)
            End If
        End Set
    End Property

    Private Const FileExtensionKey As String = "DocIconJob_FileExtensionKey"
    Private Property _fileExtension() As String
        Get
            If Properties.ContainsKey(FileExtensionKey) Then
                Return Convert.ToString(Properties(FileExtensionKey))
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If Properties.ContainsKey(FileExtensionKey) Then
                Properties(FileExtensionKey) = value
            Else
                Properties.Add(FileExtensionKey, value)
            End If
        End Set
    End Property

    Private Const ImageFilenameKey As String = "DocIconJob_ImageFilenameKey"
    Private Property _imageFilename() As String
        Get
            If Properties.ContainsKey(ImageFilenameKey) Then
                Return Convert.ToString(Properties(ImageFilenameKey))
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If Properties.ContainsKey(ImageFilenameKey) Then
                Properties(ImageFilenameKey) = value
            Else
                Properties.Add(ImageFilenameKey, value)
            End If
        End Set
    End Property

#End Region

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(JobName As String, service As SPService, Installing As Boolean, FileExtension As String, ImageFilename As String)
        MyBase.New(JobName, service)
        _installing = Installing
        _fileExtension = FileExtension
        _imageFilename = ImageFilename
    End Sub

    Public Overrides Sub Execute(jobState As Microsoft.SharePoint.Administration.SPJobState)
        UpdateDocIcon()
    End Sub

    Private Sub UpdateDocIcon()
        Dim x As New XmlDocument
        x.Load(DocIconPath)

        Dim mapNode As XmlNode = x.SelectSingleNode(String.Format("DocIcons/ByExtension/Mapping[@Key='{0}']", _fileExtension))

        If _installing Then
            'Create DocIcon entry
            If mapNode Is Nothing Then
                'Create Attributes
                Dim keyAttribute As XmlAttribute = x.CreateAttribute("Key")
                keyAttribute.Value = _fileExtension
                Dim valueAttribute As XmlAttribute = x.CreateAttribute("Value")
                valueAttribute.Value = _imageFilename

                'Create Node
                mapNode = x.CreateElement("Mapping")
                mapNode.Attributes.Append(keyAttribute)
                mapNode.Attributes.Append(valueAttribute)

                Dim byExtensionNode = x.SelectSingleNode("DocIcons/ByExtension")
                Dim NodeAdded As Boolean = False
                If byExtensionNode IsNot Nothing Then
                    'Add in alphabetic order
                    For Each mapping As XmlNode In byExtensionNode.ChildNodes
                        If mapping.Attributes("Key").Value.CompareTo(_fileExtension) > 0 Then
                            byExtensionNode.InsertBefore(mapNode, mapping)
                            NodeAdded = True
                            Exit For
                        End If
                    Next

                    If Not NodeAdded Then byExtensionNode.AppendChild(mapNode)
                    x.Save(DocIconPath)
                End If
            End If
        Else
            'Remove DocIcon entry
            If mapNode IsNot Nothing Then
                Dim byExtensionNode = x.SelectSingleNode("DocIcons/ByExtension")
                If byExtensionNode IsNot Nothing Then
                    byExtensionNode.RemoveChild(mapNode)
                    x.Save(DocIconPath)
                End If
            End If
        End If
    End Sub

End Class
