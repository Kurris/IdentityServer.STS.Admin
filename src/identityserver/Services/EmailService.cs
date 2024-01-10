using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityServer.STS.Admin.Services;

/// <summary>
/// 基于MailKit的邮件帮助类
/// </summary>
public class EmailService
{
    private readonly MailkitOptions _mailkitOptions;

    public EmailService(IOptions<MailkitOptions> options)
    {
        _mailkitOptions = options.Value;
    }


    /// <summary>
    /// 发送电子邮件
    /// </summary>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    /// <param name="toAddresses">接收方信息</param>
    /// <param name="options">发送人账号密码</param>
    public async Task SendEmailAsync(string subject, string content, IEnumerable<MailboxAddress> toAddresses, MailkitOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        await SendEmailAsync(subject, content, toAddresses, null, options);
    }


    /// <summary>
    /// 发送电子邮件
    /// </summary>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    /// <param name="toAddress">接收方信息</param>
    /// <returns></returns>
    public async Task SendEmailAsync(string subject, string content, IEnumerable<MailboxAddress> toAddress)
    {
        await SendEmailAsync(subject, content, toAddress, null, null);
    }


    /// <summary>
    /// 发送电子邮件
    /// </summary>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    /// <param name="toAddresses">接收方信息</param>
    /// <param name="attachments">附件</param>
    /// <returns></returns>
    public async Task SendEmailAsync(string subject, string content, IEnumerable<MailboxAddress> toAddresses, IEnumerable<AttachmentInfo> attachments)
    {
        await SendEmailAsync(subject, content, toAddresses, attachments, null);
    }


    /// <summary>
    /// 发送电子邮件
    /// </summary>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    /// <param name="toAddresses">接收方信息</param>
    /// <param name="attachments">附件</param>
    /// <param name="options">发送人账号密码</param>
    public async Task SendEmailAsync(string subject
        , string content
        , IEnumerable<MailboxAddress> toAddresses
        , IEnumerable<AttachmentInfo> attachments
        , MailkitOptions options)
    {
        options ??= _mailkitOptions;
        if (options == null) throw new ArgumentNullException(nameof(options));


        using (var message = new MimeMessage())
        {
            message.Subject = subject;

            message.From.Add(new MailboxAddress(Encoding.UTF8, options.UserName, options.UserAddress));
            message.To.AddRange(toAddresses);

            var builder = new BodyBuilder
            {
                HtmlBody = content
            };

            if (attachments?.Any() == true)
            {
                foreach (var att in attachments)
                {
                    var attachment = string.IsNullOrWhiteSpace(att.ContentType)
                        ? new MimePart()
                        : new MimePart(att.ContentType);

                    attachment.Content = new MimeContent(att.Stream);
                    attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                    attachment.ContentTransferEncoding = ContentEncoding.Default;
                    attachment.FileName = ConvertHeaderToBase64(att.FileName, Encoding.UTF8); //解决附件中文名问题

                    builder.Attachments.Add(attachment);
                }
            }

            message.Body = builder.ToMessageBody();
            message.Date = DateTime.Now;

            using (var client = new SmtpClient())
            {
                //创建连接
                await client.ConnectAsync(options.Host, options.Port, options.UseSsl);
                await client.AuthenticateAsync(options.UserAddress, options.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }

    private static string ConvertHeaderToBase64(string inputStr, Encoding encoding)
    {
        var encode = !string.IsNullOrEmpty(inputStr) && inputStr.Any(c => c > 127);
        return encode ? string.Concat("=?", encoding.WebName, "?B?", Convert.ToBase64String(encoding.GetBytes(inputStr)), "?=") : inputStr;
    }
}

/// <summary>
/// 附件信息
/// </summary>
public class AttachmentInfo : IDisposable
{
    public AttachmentInfo(string fileName, byte[] data, string contentType)
    {
        this.FileName = fileName;
        this.ContentType = contentType;
        this.Stream = new MemoryStream(data);
    }


    public AttachmentInfo(Stream stream, string contentType)
    {
        this.ContentType = contentType;
        this.Stream = stream;
    }

    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string ContentType { get; }


    /// <summary>
    /// 文件数据流，获取数据时优先采用此部分
    /// </summary>
    public Stream Stream { get; private set; }


    /// <summary>
    /// 释放Stream
    /// </summary>
    public void Dispose()
    {
        this.Stream?.Dispose();
        this.Stream = null;
    }
}

/// <summary>
/// 邮件参数类
/// </summary>
public class MailkitOptions
{
    /// <summary>
    /// 邮件服务器Host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 邮件服务器Port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 邮件服务器是否是ssl
    /// </summary>
    public bool UseSsl { get; set; }

    /// <summary>
    /// 发送邮件的账号名称
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 发送邮件的账号地址
    /// </summary>
    public string UserAddress { get; set; }

    /// <summary>
    /// 发现邮件所需的账号密码
    /// </summary>
    public string Password { get; set; }
}