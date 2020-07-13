Imports System.ComponentModel
Imports System.Runtime.Serialization

Partial Public Class TDDatabaseDataSet
    Public Overrides ReadOnly Property Container As IContainer
        Get
            Return MyBase.Container
        End Get
    End Property

    Public Overrides ReadOnly Property DesignMode As Boolean
        Get
            Return MyBase.DesignMode
        End Get
    End Property

    Public Overrides Property Site As ISite
        Get
            Return MyBase.Site
        End Get
        Set(value As ISite)
            MyBase.Site = value
        End Set
    End Property

    Partial Public Class VisitesTDDataTable
        Private Sub VisitesTDDataTable_VisitesTDRowChanging(sender As Object, e As VisitesTDRowChangeEvent) Handles Me.VisitesTDRowChanging

        End Sub
    End Class

    Public Sub New(dataSetName As String)
        MyBase.New(dataSetName)
    End Sub

    Protected Sub New(info As SerializationInfo, context As StreamingContext, ConstructSchema As Boolean)
        MyBase.New(info, context, ConstructSchema)
    End Sub

    Public Overrides Function Equals(obj As Object) As Boolean
        Return MyBase.Equals(obj)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return MyBase.GetHashCode()
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    Public Overrides Function GetService(service As Type) As Object
        Return MyBase.GetService(service)
    End Function

    Public Overrides Function ToString() As String
        Return MyBase.ToString()
    End Function

    Public Overrides Sub GetObjectData(info As SerializationInfo, context As StreamingContext)
        MyBase.GetObjectData(info, context)
    End Sub

    Protected Overrides Sub OnPropertyChanging(pcevent As PropertyChangedEventArgs)
        MyBase.OnPropertyChanging(pcevent)
    End Sub

    Protected Overrides Sub OnRemoveTable(table As DataTable)
        MyBase.OnRemoveTable(table)
    End Sub

    Protected Overrides Sub OnRemoveRelation(relation As DataRelation)
        MyBase.OnRemoveRelation(relation)
    End Sub

    Public Overrides Sub RejectChanges()
        MyBase.RejectChanges()
    End Sub

    Public Overrides Sub Reset()
        MyBase.Reset()
    End Sub

    Public Overrides Sub Load(reader As IDataReader, loadOption As LoadOption, errorHandler As FillErrorEventHandler, ParamArray tables() As DataTable)
        MyBase.Load(reader, loadOption, errorHandler, tables)
    End Sub
End Class
