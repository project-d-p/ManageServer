// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/matching.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Matching {

  /// <summary>Holder for reflection information generated from Protos/matching.proto</summary>
  public static partial class MatchingReflection {

    #region Descriptor
    /// <summary>File descriptor for Protos/matching.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MatchingReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVQcm90b3MvbWF0Y2hpbmcucHJvdG8SCG1hdGNoaW5nIiAKC1RlYW1QbGF5",
            "ZXJzEhEKCXBsYXllcklkcxgBIAMoCSJQCg5TZXJ2ZXJSZXNwb25zZRINCgVV",
            "ZHBJcBgBIAEoCRIPCgdVZHBQb3J0GAIgASgJEg0KBVRjcElwGAMgASgJEg8K",
            "B1RjcFBvcnQYBCABKAkyVQoPTWF0Y2hpbmdTZXJ2aWNlEkIKD1NlbmRUZWFt",
            "UGxheWVycxIVLm1hdGNoaW5nLlRlYW1QbGF5ZXJzGhgubWF0Y2hpbmcuU2Vy",
            "dmVyUmVzcG9uc2ViBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Matching.TeamPlayers), global::Matching.TeamPlayers.Parser, new[]{ "PlayerIds" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Matching.ServerResponse), global::Matching.ServerResponse.Parser, new[]{ "UdpIp", "UdpPort", "TcpIp", "TcpPort" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// 팀 정보를 담는 메시지
  /// </summary>
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class TeamPlayers : pb::IMessage<TeamPlayers>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<TeamPlayers> _parser = new pb::MessageParser<TeamPlayers>(() => new TeamPlayers());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<TeamPlayers> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Matching.MatchingReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TeamPlayers() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TeamPlayers(TeamPlayers other) : this() {
      playerIds_ = other.playerIds_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TeamPlayers Clone() {
      return new TeamPlayers(this);
    }

    /// <summary>Field number for the "playerIds" field.</summary>
    public const int PlayerIdsFieldNumber = 1;
    private static readonly pb::FieldCodec<string> _repeated_playerIds_codec
        = pb::FieldCodec.ForString(10);
    private readonly pbc::RepeatedField<string> playerIds_ = new pbc::RepeatedField<string>();
    /// <summary>
    /// 팀에 속한 플레이어의 ID 리스트
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> PlayerIds {
      get { return playerIds_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as TeamPlayers);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(TeamPlayers other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!playerIds_.Equals(other.playerIds_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= playerIds_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      playerIds_.WriteTo(output, _repeated_playerIds_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      playerIds_.WriteTo(ref output, _repeated_playerIds_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      size += playerIds_.CalculateSize(_repeated_playerIds_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(TeamPlayers other) {
      if (other == null) {
        return;
      }
      playerIds_.Add(other.playerIds_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            playerIds_.AddEntriesFrom(input, _repeated_playerIds_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            playerIds_.AddEntriesFrom(ref input, _repeated_playerIds_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  /// <summary>
  /// 서버 응답 메시지
  /// </summary>
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class ServerResponse : pb::IMessage<ServerResponse>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ServerResponse> _parser = new pb::MessageParser<ServerResponse>(() => new ServerResponse());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ServerResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Matching.MatchingReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerResponse(ServerResponse other) : this() {
      udpIp_ = other.udpIp_;
      udpPort_ = other.udpPort_;
      tcpIp_ = other.tcpIp_;
      tcpPort_ = other.tcpPort_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerResponse Clone() {
      return new ServerResponse(this);
    }

    /// <summary>Field number for the "UdpIp" field.</summary>
    public const int UdpIpFieldNumber = 1;
    private string udpIp_ = "";
    /// <summary>
    /// UDP_IP 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string UdpIp {
      get { return udpIp_; }
      set {
        udpIp_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "UdpPort" field.</summary>
    public const int UdpPortFieldNumber = 2;
    private string udpPort_ = "";
    /// <summary>
    /// UDP_PORT
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string UdpPort {
      get { return udpPort_; }
      set {
        udpPort_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "TcpIp" field.</summary>
    public const int TcpIpFieldNumber = 3;
    private string tcpIp_ = "";
    /// <summary>
    /// TCP_IP 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string TcpIp {
      get { return tcpIp_; }
      set {
        tcpIp_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "TcpPort" field.</summary>
    public const int TcpPortFieldNumber = 4;
    private string tcpPort_ = "";
    /// <summary>
    /// TCP_PORT
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string TcpPort {
      get { return tcpPort_; }
      set {
        tcpPort_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ServerResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ServerResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (UdpIp != other.UdpIp) return false;
      if (UdpPort != other.UdpPort) return false;
      if (TcpIp != other.TcpIp) return false;
      if (TcpPort != other.TcpPort) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (UdpIp.Length != 0) hash ^= UdpIp.GetHashCode();
      if (UdpPort.Length != 0) hash ^= UdpPort.GetHashCode();
      if (TcpIp.Length != 0) hash ^= TcpIp.GetHashCode();
      if (TcpPort.Length != 0) hash ^= TcpPort.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (UdpIp.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(UdpIp);
      }
      if (UdpPort.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(UdpPort);
      }
      if (TcpIp.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(TcpIp);
      }
      if (TcpPort.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(TcpPort);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (UdpIp.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(UdpIp);
      }
      if (UdpPort.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(UdpPort);
      }
      if (TcpIp.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(TcpIp);
      }
      if (TcpPort.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(TcpPort);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (UdpIp.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(UdpIp);
      }
      if (UdpPort.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(UdpPort);
      }
      if (TcpIp.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(TcpIp);
      }
      if (TcpPort.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(TcpPort);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ServerResponse other) {
      if (other == null) {
        return;
      }
      if (other.UdpIp.Length != 0) {
        UdpIp = other.UdpIp;
      }
      if (other.UdpPort.Length != 0) {
        UdpPort = other.UdpPort;
      }
      if (other.TcpIp.Length != 0) {
        TcpIp = other.TcpIp;
      }
      if (other.TcpPort.Length != 0) {
        TcpPort = other.TcpPort;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            UdpIp = input.ReadString();
            break;
          }
          case 18: {
            UdpPort = input.ReadString();
            break;
          }
          case 26: {
            TcpIp = input.ReadString();
            break;
          }
          case 34: {
            TcpPort = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            UdpIp = input.ReadString();
            break;
          }
          case 18: {
            UdpPort = input.ReadString();
            break;
          }
          case 26: {
            TcpIp = input.ReadString();
            break;
          }
          case 34: {
            TcpPort = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
