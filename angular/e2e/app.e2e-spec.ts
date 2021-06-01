import { MessengerTemplatePage } from './app.po';

describe('Messenger App', function() {
  let page: MessengerTemplatePage;

  beforeEach(() => {
    page = new MessengerTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
